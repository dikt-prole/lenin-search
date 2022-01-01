using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;
using JsonCommentData = LeninSearch.Standard.Core.Corpus.Json.JsonCommentData;
using JsonFileData = LeninSearch.Standard.Core.Corpus.Json.JsonFileData;
using JsonHeading = LeninSearch.Standard.Core.Corpus.Json.JsonHeading;
using JsonParagraph = LeninSearch.Standard.Core.Corpus.Json.JsonParagraph;

namespace LeninSearch.Script.Scripts
{
    public class Fb2ToJsonScript : IScript
    {
        public string Id => "fb2-json";
        public void Execute(params string[] input)
        {
            var fb2Folder = input[0];
            var jsonFolder = input[1];
            var jpegQuality = input.Length == 3 ? long.Parse(input[2]) : -1;
            foreach (var f in Directory.GetFiles(jsonFolder)) File.Delete(f);

            var fb2Files = Directory.GetFiles(fb2Folder, "*.fb2");
            var imageIds = new List<string>();
            foreach (var fb2File in fb2Files)
            {
                var jsonFileData = new JsonFileData
                {
                    Pars = new List<JsonParagraph>(),
                    Headings = new List<JsonHeading>(),
                    Pages = new Dictionary<ushort, ushort>()
                };

                var fileName = Path.GetFileName(fb2File).Replace(".fb2", "");
                var jsonFile = Path.Combine(jsonFolder, $"{fileName}.json");
                Console.WriteLine($"Generate: {jsonFile}");

                var fb2Xml = File.ReadAllText(fb2File)
                    .Replace("l:href", "lhref")
                    .Replace("xlink:href", "lhref")
                    .Replace("\r", "")
                    .Replace("\n", "")
                    .Replace("  ", " ")
                    .Replace("> ", ">");
                while (fb2Xml.Contains("<emphasis><emphasis>"))
                    fb2Xml = fb2Xml.Replace("<emphasis><emphasis>", "<emphasis>");
                while (fb2Xml.Contains("</emphasis></emphasis>"))
                    fb2Xml = fb2Xml.Replace("</emphasis></emphasis>", "</emphasis>");
                while (fb2Xml.Contains("<strong><strong>"))
                    fb2Xml = fb2Xml.Replace("<strong><strong>", "<strong>");
                while (fb2Xml.Contains("</strong></strong>"))
                    fb2Xml = fb2Xml.Replace("</strong></strong>", "</strong>");

                var bodyStart = fb2Xml.IndexOf("<body");
                var bodyEnd = fb2Xml.IndexOf("</body>");
                var bodyXml = fb2Xml.Substring(bodyStart, bodyEnd - bodyStart + 7)
                    .Replace("<section>", "")
                    .Replace("</section>", "");
                var bodyDoc = new XmlDocument();
                bodyDoc.LoadXml(bodyXml);
                var bodyRoot = bodyDoc.DocumentElement;
                var commentIds = new List<string>();
                
                foreach (XmlNode node in bodyRoot.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "image":
                            jsonFileData.Pars.Add(XmlToJsonUtil.GetImageParagraph(node, imageIds, id => $"{fileName}{id}"));
                            break;
                        case "title":
                            var titleParagraph = XmlToJsonUtil.GetTitleParagraph(node, commentIds, imageIds, id => $"{fileName}{id}");
                            jsonFileData.Pars.Add(titleParagraph);
                            jsonFileData.Headings.Add(new JsonHeading
                            {
                                Index = (ushort)(jsonFileData.Pars.Count - 1),
                                Level = 0,
                                Text = titleParagraph.Text
                            });
                            break;
                        case "cite":
                        case "p":
                            if (node.ChildNodes.Count > 0 && node.ChildNodes[0].Name == "image")
                            {
                                jsonFileData.Pars.Add(XmlToJsonUtil.GetImageParagraph(node.ChildNodes[0], imageIds, id => $"{fileName}{id}"));
                            }
                            else
                            {
                                jsonFileData.Pars.Add(XmlToJsonUtil.GetNormalParagraph(node, commentIds, imageIds, id => $"{fileName}{id}"));
                            }
                            break;
                    }
                }

                //var binaryStartIndex = fb2Xml.IndexOf("<binary");
                //var binaryEndIndex = fb2Xml.LastIndexOf("</binary>") + "</binary>".Length;
                //var binaryXml = $"<fb2>{fb2Xml.Substring(binaryStartIndex, binaryEndIndex - binaryStartIndex)}</fb2>";
                var fb2Doc = new XmlDocument();
                fb2Doc.LoadXml(fb2Xml);

                var comments = new Dictionary<string, JsonCommentData>();
                foreach (var cd in jsonFileData.Pars.SelectMany(p => p.Comments ?? new List<JsonCommentData>()))
                {
                    if (comments.ContainsKey(cd.CommentId)) continue;
                    comments.Add(cd.CommentId, cd);
                }
                foreach (XmlNode node in fb2Doc.DocumentElement.ChildNodes)
                {
                    if (node.Name == "binary")
                    {
                        var imageId = $"{fileName}{node.Attributes["id"].Value}";
                        var imageIndex = imageIds.IndexOf(imageId);
                        if (imageIndex > 0)
                        {
                            var imageBase64 = node.InnerText;
                            var imageBytes = Convert.FromBase64String(imageBase64);
                            var imageFile = Path.Combine(jsonFolder, $"image{imageIndex}.jpeg");
                            var tempFile = Path.GetTempFileName() + ".jpeg";
                            File.WriteAllBytes(tempFile, imageBytes);
                            using (var image = Image.FromFile(tempFile))
                            {
                                if (jpegQuality > 0)
                                {
                                    var encoderParams = new EncoderParameters(1);
                                    var qualityParam = new EncoderParameter(Encoder.Quality, jpegQuality);
                                    var jpegEncoder = ImageCodecInfo.GetImageEncoders().First(e => e.MimeType == "image/jpeg");
                                    encoderParams.Param[0] = qualityParam;
                                    image.Save(imageFile, jpegEncoder, encoderParams);
                                }
                                else
                                {
                                    image.Save(imageFile, ImageFormat.Jpeg);
                                }
                            }
                        }
                    }
                    else if (node.Name == "body" && node.Attributes["name"]?.Value == "notes")
                    {
                        var noteSections = node.ChildNodes.OfType<XmlNode>().SelectMany(n => new[] { n }.Concat(n.ChildNodes.OfType<XmlNode>())).ToList();
                        foreach (var noteSection in noteSections)
                        {
                            var commentId = noteSection.Attributes["id"]?.Value;

                            if (commentId == null || !comments.ContainsKey(commentId)) continue;

                            comments[commentId].Text = noteSection.InnerText;
                        }
                    }
                }

                File.WriteAllText(jsonFile, JsonConvert.SerializeObject(jsonFileData, Formatting.Indented));
            }
        }
    }
}