using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using LeninSearch.Standard.Core;
using LeninSearch.Standard.Core.Corpus.Json;

namespace LeninSearch.Script
{
    public static class XmlToJsonUtil
    {
        public static JsonParagraph GetImageParagraph(XmlNode node, List<string> imageIds, Func<string, string> imageIdFunc)
        {
            var lhref = node.Attributes["lhref"].InnerText;
            var imageId = imageIdFunc(lhref.Replace("#", ""));
            imageIds.Add(imageId);
            return new JsonParagraph
            {
                Text = $"image{imageIds.Count - 1}",
                ParagraphType = JsonParagraphType.Illustration,
                ImageIndex = (ushort)(imageIds.Count - 1)
            };
        }

        public static JsonParagraph GetNormalParagraph(XmlNode node, List<string> commentIds, List<string> imageIds, Func<string, string> imageIdFunc)
        {
            var xmlTextData = GetXmlTextData(node.InnerXml, commentIds, imageIds, imageIdFunc);
            return new JsonParagraph
            {
                Text = xmlTextData.Text,
                Comments = xmlTextData.Comments,
                Markups = xmlTextData.Markups,
                ParagraphType = JsonParagraphType.Normal,
                InlineImages  = xmlTextData.Images
            };
        }

        public static JsonParagraph GetTitleParagraph(XmlNode node, List<string> commentIds, List<string> imageIds, Func<string, string> imageIdFunc)
        {
            var xml = node.InnerXml.Replace("<p>", "").Replace("</p>", " ").TrimEnd(' ');
            xml = TextUtil.Trim(xml, "<p", ">");

            var xmlTextData = GetXmlTextData(xml, commentIds, imageIds, imageIdFunc);
            return new JsonParagraph
            {
                Text = xmlTextData.Text,
                Comments = xmlTextData.Comments,
                Markups = xmlTextData.Markups,
                ParagraphType = JsonParagraphType.Heading0,
                InlineImages = xmlTextData.Images
            };
        }

        private static (List<JsonMarkupData> Markups, List<JsonCommentData> Comments, List<JsonInlineImageData> Images, string Text) GetXmlTextData(string xml, List<string> commentIds, List<string> imageIds, Func<string, string> imageIdFunc)
        {
            var text = xml;
            var markups = new List<JsonMarkupData>();
            var comments = new List<JsonCommentData>();
            var images = new List<JsonInlineImageData>();

            while (true)
            {
                var tagIndexes = new (string OpenTag, string CloseTag, int Index)[]
                {
                    ("<strong>", "</strong>", text.IndexOf("<strong>")),
                    ("<emphasis>","</emphasis>", text.IndexOf("<emphasis>")),
                    ("<a ","</a>", text.IndexOf("<a ")),
                    ("<image ", "/>", text.IndexOf("<image "))
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
                else if (tagNode.Name == "a")
                {
                    var commentId = tagNode.Attributes["lhref"].Value.Replace("#", "");
                    commentIds.Add(commentId);
                    comments.Add(new JsonCommentData(commentId, (ushort)(commentIds.Count - 1), (ushort)tagStart));
                    text = text.Substring(0, tagStart) + text.Substring(tagEnd);
                }
                else // inline image
                {
                    var imageId = tagNode.Attributes["lhref"].Value.Replace("#", "");

                    var before = text.Substring(0, tagStart);
                    if (!before.EndsWith(" ")) before = $"{before} ";

                    var after = text.Substring(tagEnd);
                    if (!after.StartsWith(" ")) after = $" {after}";
                    
                    imageId = imageIdFunc(imageId);
                    imageIds.Add(imageId);
                    var imageIndex = (ushort)(imageIds.Count - 1);
                    var inlineImage = new JsonInlineImageData(imageIndex, (ushort) before.Length);
                    images.Add(inlineImage);
                    text = before + inlineImage.ImageToken + after;
                }
            }

            return (markups, comments, images, text);
        }
    }
}