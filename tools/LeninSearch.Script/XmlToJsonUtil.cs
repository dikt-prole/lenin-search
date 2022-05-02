using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

                var tagIndex = tagIndexes[0];

                var tagSpan = GetTagSpan(text, tagIndex.OpenTag, tagIndex.CloseTag);
                var tagXml = text.Substring(tagSpan.TagStart, tagSpan.TagEnd - tagSpan.TagStart);

                var tagDoc = new XmlDocument();
                tagDoc.LoadXml($"<wrap>{tagXml}</wrap>");
                var tagNode = tagDoc.DocumentElement.ChildNodes[0];

                if (tagNode.Name == "strong")
                {
                    var tagText = tagNode.InnerText;
                    markups.Add(JsonMarkupData.Strong(tagSpan.TagStart, (ushort)tagText.Length));
                    text = text.Substring(0, tagSpan.TagStart) + tagText + text.Substring(tagSpan.TagEnd);
                }
                else if (tagNode.Name == "emphasis")
                {
                    var tagText = tagNode.InnerText;
                    markups.Add(JsonMarkupData.Emphasis(tagSpan.TagStart, (ushort)tagText.Length));
                    text = text.Substring(0, tagSpan.TagStart) + tagText + text.Substring(tagSpan.TagEnd);
                }
                else if (tagNode.Name == "a")
                {
                    var commentId = tagNode.Attributes["lhref"].Value.Replace("#", "");
                    commentIds.Add(commentId);
                    var commentIndex = (ushort) (commentIds.Count - 1);
                    var comment = new JsonCommentData(commentId, commentIndex, tagSpan.TagStart);
                    comments.Add(comment);
                    text = text.Substring(0, tagSpan.TagStart) + $" {comment.Token} " + text.Substring(tagSpan.TagEnd);
                }
                else // inline image
                {
                    var imageId = tagNode.Attributes["lhref"].Value.Replace("#", "");

                    var before = text.Substring(0, tagSpan.TagStart);
                    if (!before.EndsWith(" ")) before = $"{before} ";

                    var after = text.Substring(tagSpan.TagEnd);
                    if (!after.StartsWith(" ")) after = $" {after}";
                    
                    imageId = imageIdFunc(imageId);
                    imageIds.Add(imageId);
                    var imageIndex = (ushort)(imageIds.Count - 1);
                    var inlineImage = new JsonInlineImageData(imageIndex, (ushort) before.Length);
                    images.Add(inlineImage);
                    text = before + $" {inlineImage.Token} " + after;
                }
            }

            return (markups, comments, images, text);
        }

        public static XmlNode GetCommentNode(string xml, string commentId)
        {
            var startToken = $"<section id=\"{commentId}\"";
            var startIndex = xml.IndexOf(startToken);
            if (startIndex == -1) return null;
            var endToken = "</section>";
            var endIndex = xml.IndexOf(endToken, startIndex);
            if (endIndex == -1) return null;
            endIndex += endToken.Length;
            var sectinoXml = xml.Substring(startIndex, endIndex - startIndex);
            var sectinoDoc = new XmlDocument();
            sectinoDoc.LoadXml(sectinoXml);
            return sectinoDoc.DocumentElement;
        }

        private static (ushort TagStart, ushort TagEnd) GetTagSpan(string text, string openTag, string closeTag)
        {
            var tagStart = text.IndexOf(openTag);
            var tagEnd = text.IndexOf(closeTag, tagStart) + closeTag.Length;

            while (true)
            {
                var tagLength = tagEnd - tagStart;
                if (tagLength < 0)
                {
                    throw new Exception($"Invalid tag length: {tagLength}");
                }

                var containsUnclosed = ContainsUnclosedTags(text.Substring(tagStart, tagLength));

                if (!containsUnclosed) break;

                tagEnd = text.IndexOf(closeTag, tagEnd) + closeTag.Length;
            }

            return ((ushort)tagStart, (ushort)tagEnd);
        }

        private static bool ContainsUnclosedTags(string xml)
        {
            var tagOptions = new (string OpenTag, string CloseTag)[]
            {
                ("<strong>", "</strong>"),
                ("<emphasis>", "</emphasis>")
            };

            foreach (var tagOption in tagOptions)
            {
                var openCount = new Regex(tagOption.OpenTag).Matches(xml).Count;

                var closeCount = new Regex(tagOption.CloseTag).Matches(xml).Count;

                if (openCount != closeCount) return true;
            }

            return false;
        }
    }
}