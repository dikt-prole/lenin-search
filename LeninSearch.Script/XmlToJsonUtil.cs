using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using LeninSearch.Standard.Core.Corpus.Json;

namespace LeninSearch.Script
{
    public static class XmlToJsonUtil
    {
        public static JsonParagraph GetImageParagraph(XmlNode node, List<string> images, Func<string, string> imageIdFunc)
        {
            var lhref = node.Attributes["lhref"].InnerText;
            var imageId = imageIdFunc(lhref.Replace("#", ""));
            images.Add(imageId);
            return new JsonParagraph
            {
                Text = $"image{images.Count - 1}",
                ParagraphType = JsonParagraphType.Illustration,
                ImageIndex = (ushort)(images.Count - 1)
            };
        }

        public static JsonParagraph GetNormalParagraph(XmlNode node, List<string> commentIds)
        {
            var xmlTextData = GetXmlTextData(node.InnerXml, commentIds);
            return new JsonParagraph
            {
                Text = xmlTextData.Text,
                Comments = xmlTextData.Comments,
                Markups = xmlTextData.Markups,
                ParagraphType = JsonParagraphType.Normal
            };
        }

        public static JsonParagraph GetTitleParagraph(XmlNode node, List<string> commentIds)
        {
            var xml = node.InnerXml.Replace("<p>", "").Replace("</p>", " ").TrimEnd(' ');
            var xmlTextData = GetXmlTextData(xml, commentIds);
            return new JsonParagraph
            {
                Text = xmlTextData.Text,
                Comments = xmlTextData.Comments,
                Markups = xmlTextData.Markups,
                ParagraphType = JsonParagraphType.Heading0
            };
        }

        private static (List<JsonMarkupData> Markups, List<JsonCommentData> Comments, string Text) GetXmlTextData(string xml, List<string> commentIds)
        {
            var text = xml;
            var markups = new List<JsonMarkupData>();
            var comments = new List<JsonCommentData>();

            while (true)
            {
                var tagIndexes = new (string OpenTag, string CloseTag, int Index)[]
                {
                    ("<strong>", "</strong>", text.IndexOf("<strong>")),
                    ("<emphasis>","</emphasis>", text.IndexOf("<emphasis>")),
                    ("<a ","</a>", text.IndexOf("<a "))
                }
                    .Where(ti => ti.Index != -1)
                    .OrderBy(ti => ti.Index)
                    .ToList();

                if (tagIndexes.Count == 0) break;

                var tagStart = tagIndexes[0].Index;
                var tagEnd = text.IndexOf(tagIndexes[0].CloseTag, tagStart) + tagIndexes[0].CloseTag.Length;
                var tagXml = text.Substring(tagStart, tagEnd - tagStart);

                var tagDoc = new XmlDocument();
                tagDoc.LoadXml($"<wrap>{tagXml}</wrap>");
                var tagNode = tagDoc.DocumentElement.ChildNodes[0];

                if (tagNode.Name == "strong")
                {
                    var tagText = tagNode.InnerText;
                    markups.Add(JsonMarkupData.Strong((ushort)tagStart, (ushort)tagText.Length));
                    text = text.Substring(0, tagStart) + tagText + text.Substring(tagEnd);
                }
                else if (tagNode.Name == "emphasis")
                {
                    var tagText = tagNode.InnerText;
                    markups.Add(JsonMarkupData.Emphasis((ushort)tagStart, (ushort)tagText.Length));
                    text = text.Substring(0, tagStart) + tagText + text.Substring(tagEnd);
                }
                else
                {
                    var commentId = tagNode.Attributes["lhref"].Value.Replace("#", "");
                    commentIds.Add(commentId);
                    comments.Add(new JsonCommentData(commentId, (ushort)(commentIds.Count - 1), (ushort)tagStart));
                    text = text.Substring(0, tagStart) + text.Substring(tagEnd);
                }
            }

            return (markups, comments, text);
        }
    }
}