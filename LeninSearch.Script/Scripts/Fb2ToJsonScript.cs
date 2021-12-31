using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Xml;
using LeninSearch.Standard.Core.Corpus;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace LeninSearch.Script.Scripts
{
    public class Fb2ToJsonScript : IScript
    {
        public string Id => "fb2-json";
        public void Execute(params string[] input)
        {
            var fb2Folder = input[0];
            var jsonFolder = input[1];

            foreach (var f in Directory.GetFiles(jsonFolder)) File.Delete(f);

            var images = new List<string>();

            var fb2Files = Directory.GetFiles(fb2Folder, "*.fb2");
            foreach (var fb2File in fb2Files)
            {
                var fileData = new JsonFileData
                {
                    Pars = new List<JsonParagraph>(),
                    Headings = new List<JsonHeading>(),
                    Pages = new Dictionary<ushort, ushort>()
                };

                var fileName = Path.GetFileName(fb2File).Replace(".fb2", "");
                var fileDataFile = Path.Combine(jsonFolder, $"{fileName}.json");
                Console.WriteLine($"Generate: {fileDataFile}");

                var fb2Xml = File.ReadAllText(fb2File).Replace("\r", "").Replace("\n", "").Replace("  ", " ").Replace("> ", ">");
                var bodyStart = fb2Xml.IndexOf("<body");
                var bodyEnd = fb2Xml.IndexOf("</body");

                // 1. construct paragraphs and headings
                var pStartIndex = bodyStart;
                ushort pIndex = 0;
                while (true)
                {
                    pStartIndex = fb2Xml.IndexOf("<p>", pStartIndex) + 3;

                    if (pStartIndex == -1 || pStartIndex > bodyEnd) break;

                    var pEndIndex = fb2Xml.IndexOf("</p>", pStartIndex);

                    if (pEndIndex == -1 || pEndIndex > bodyEnd) break;

                    var pText = fb2Xml.Substring(pStartIndex, pEndIndex - pStartIndex);

                    var pBefore = fb2Xml.Substring(pStartIndex - 13, 10);

                    var isTitle = pBefore.Contains("<title>");

                    var isImage = pText.Contains("<image");

                    if (isImage)
                    {
                        var imageFile = $"{fileName}{GetParagraphImageFile(pText)}";
                        images.Add(imageFile);
                    }

                    var imageIndex = isImage ? (ushort?)(images.Count - 1) : null;

                    var commentData = GetCommentData(pText, fb2Xml);

                    var paragraph = new JsonParagraph
                    {
                        Text = isImage 
                            ? $"image{imageIndex}.jpeg" 
                            : commentData.Comments.Any()
                                ? commentData.Text
                                : pText,
                        ImageIndex = imageIndex,
                        ParagraphType = isImage
                            ? JsonParagraphType.Illustration
                            : isTitle
                                ? JsonParagraphType.Heading0
                                : JsonParagraphType.Normal,
                        Comments = commentData.Comments,
                        Markups = new List<JsonMarkupData>()
                    };

                    if (!isImage)
                    {
                        var markupResult = GetMarkupData(paragraph.Text);
                        paragraph.Text = markupResult.Text;
                        paragraph.Markups = markupResult.Markups;
                    }

                    fileData.Pars.Add(paragraph);

                    if (isTitle)
                    {
                        var heading = new JsonHeading
                        {
                            Text = pText,
                            Index = pIndex,
                            Level = 0
                        };

                        fileData.Headings.Add(heading);
                    }

                    pIndex++;
                }

                var fileDataJson = JsonConvert.SerializeObject(fileData, Formatting.Indented);
                
                File.WriteAllText(fileDataFile, fileDataJson);

                // 2. save images
                var binaryStart = 0;
                while (true)
                {
                    binaryStart = fb2Xml.IndexOf("<binary", binaryStart + 1);

                    if (binaryStart == -1) break;;

                    var binaryEnd = fb2Xml.IndexOf("</binary>", binaryStart);
                    var binaryText = fb2Xml.Substring(binaryStart, binaryEnd - binaryStart);
                    var imageFile = $"{fileName}{GetBinaryImageFile(binaryText)}";
                    Console.WriteLine($"Saving '{imageFile}'");

                    var binaryBase64 = binaryText.Substring(binaryText.IndexOf('>') + 1);
                    var binaryBytes = Convert.FromBase64String(binaryBase64);
                    
                    var tempFile = Path.GetTempFileName() + ".jpeg";
                    File.WriteAllBytes(tempFile, binaryBytes);

                    var imageIndex = images.IndexOf(imageFile);

                    if (imageIndex < 0) continue;

                    var binaryFile = Path.Combine(jsonFolder, $"image{imageIndex}.jpeg");
                    var encoderParams = new EncoderParameters(1);
                    var qualityParam = new EncoderParameter(Encoder.Quality, 10L);
                    var jpegEncoder = ImageCodecInfo.GetImageEncoders().First(e => e.MimeType == "image/jpeg");
                    encoderParams.Param[0] = qualityParam;
                    using (var image = Image.FromFile(tempFile))
                    {
                        image.Save(binaryFile, jpegEncoder, encoderParams);
                    }
                }
            }
        }

        private string GetParagraphImageFile(string pText)
        {
            var start = pText.IndexOf('#') + 1;
            var end = pText.IndexOf('"', start);
            return pText.Substring(start, end - start);
        }

        private string GetBinaryImageFile(string binaryText)
        {
            var start = binaryText.IndexOf("id=") + 4;
            var end = binaryText.IndexOf('"', start);
            return binaryText.Substring(start, end - start);
        }
        
        private const string CommentStartToken = "<a l:href=\"#";
        private (List<JsonCommentData> Comments, string Text) GetCommentData(string pText, string fb2Xml)
        {
            var comments = new List<JsonCommentData>();
            var text = pText;
            byte commentIndex = 0;

            var cStart = 0;
            while (true)
            {
                cStart = text.IndexOf(CommentStartToken, cStart);

                if (cStart == -1) break;

                cStart = cStart + CommentStartToken.Length;

                var cEnd = text.IndexOf('"', cStart);

                var commentId = text.Substring(cStart, cEnd - cStart);

                var sectionTag = $"<section id=\"{commentId}\">";

                var commentSectionStart = fb2Xml.IndexOf(sectionTag);

                var commentSectionEnd = fb2Xml.IndexOf("</section>", commentSectionStart);

                var commentSectionText = fb2Xml.Substring(commentSectionStart, commentSectionEnd - commentSectionStart);

                var pStart = commentSectionText.LastIndexOf("<p>") + 3;

                var pEnd = commentSectionText.IndexOf("</p>", pStart);

                var commentText = commentSectionText.Substring(pStart, pEnd - pStart)
                    .Trim(' ', '\t', Convert.ToChar(160), Convert.ToChar(32))
                    .Trim('\t').Trim(Convert.ToChar(160));

                comments.Add(new JsonCommentData
                {
                    CommentIndex = commentIndex,
                    Text = commentText
                });

                cStart = cStart - CommentStartToken.Length;
                cEnd = text.IndexOf("</a>", cEnd) + 4;

                var beforeToken = text.Substring(0, cStart).TrimEnd(' '); ;
                if (beforeToken.Length > 0 && char.IsLetterOrDigit(beforeToken.Last()))
                {
                    beforeToken = $"{beforeToken} ";
                }

                var afterToken = text.Substring(cEnd).TrimStart(' ');
                if (afterToken.Length > 0 && char.IsLetterOrDigit(afterToken.First()))
                {
                    afterToken = $" {afterToken}";
                }

                text = $"{beforeToken}c{commentIndex}{afterToken}";

                commentIndex++;
            }

            return (comments, text);
        }

        private (List<JsonMarkupData> Markups, string Text) GetMarkupData(string pText)
        {
            var markupOptions = new List<(string StartTag, string EndTag, MarkupType MarkupType)>
            {
                ("<strong><emphasis>", "</emphasis></strong>", MarkupType.StrongEmphasis),
                ("<emphasis><strong>", "</strong></emphasis>", MarkupType.StrongEmphasis),
                ("<strong>", "</strong>", MarkupType.Strong),
                ("<emphasis>", "</emphasis>", MarkupType.Emphasis)
            };

            var markups = new List<JsonMarkupData>();
            var text = pText;

            byte markupIndex = 0;

            foreach (var mo in markupOptions)
            {
                var moStart = 0;
                while (true)
                {
                    moStart = text.IndexOf(mo.StartTag, moStart);

                    if (moStart == -1) break;

                    moStart = moStart + mo.StartTag.Length;

                    var moEnd = text.IndexOf(mo.EndTag, moStart);

                    markups.Add(new JsonMarkupData
                    {
                        MarkupIndex = markupIndex,
                        MarkupType = mo.MarkupType,
                        MarkupText = text.Substring(moStart, moEnd - moStart).Trim(' ')
                    });

                    moStart = moStart - mo.StartTag.Length;
                    moEnd = moEnd + mo.EndTag.Length;

                    var beforeToken = text.Substring(0, moStart).TrimEnd(' ');
                    //if (beforeToken.Length > 0 && char.IsLetterOrDigit(beforeToken.Last()))
                    {
                        beforeToken = $"{beforeToken} ";
                    }

                    var afterToken = text.Substring(moEnd).TrimStart(' ');
                    if (afterToken.Length > 0 && char.IsLetterOrDigit(afterToken.First()))
                    {
                        afterToken = $" {afterToken}";
                    }

                    text = $"{beforeToken}m{markupIndex}{afterToken}";

                    markupIndex++;
                }
            }

            return (markups, text);
        }
    }
}