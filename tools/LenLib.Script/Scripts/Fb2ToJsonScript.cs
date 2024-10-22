﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;
using JsonCommentData = LenLib.Standard.Core.Corpus.Json.JsonCommentData;
using JsonFileData = LenLib.Standard.Core.Corpus.Json.JsonFileData;
using JsonHeading = LenLib.Standard.Core.Corpus.Json.JsonHeading;
using JsonParagraph = LenLib.Standard.Core.Corpus.Json.JsonParagraph;

namespace LenLib.Script.Scripts
{
    public class Fb2ToJsonScript : IScript
    {
        public string Id => "fb2-json";
        public string Arguments => "1. fb2_folder, 2. json_folder, 3. keep_headings(opt), 4. jpeg-quality(opt)";

        public void Execute(params string[] input)
        {
            var fb2Folder = input[0];
            var jsonFolder = input[1];
            var keepHeadings = input.Length > 2 && input[2] == "1";
            var jpegQuality = input.Length > 3 ? long.Parse(input[3]) : -1;

            var existingJsonFiles = Directory.GetFiles(jsonFolder, "*.json");
            var existingJsonDatas = new Dictionary<string, JsonFileData>();
            foreach (var jsonFile in existingJsonFiles)
            {
                var jsonData = JsonConvert.DeserializeObject<JsonFileData>(File.ReadAllText(jsonFile));
                existingJsonDatas.Add(jsonFile, jsonData);
            }

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
                    .Replace("<sup>", "").Replace("</sup>", "")
                    .Replace("<sub>", "").Replace("</sub>", "")
                    .Replace("<cite>", "").Replace("</cite>", "")
                    .Replace("></image>", "/>")
                    .Replace("\r", "")
                    .Replace("\n", "")
                    .Replace("\t", " ");
                while (fb2Xml.Contains("  "))
                {
                    fb2Xml = fb2Xml.Replace("  ", " ");
                }

                // 1. get json paragraphs
                var bodyStart = fb2Xml.IndexOf("<body");
                var bodyEnd = fb2Xml.IndexOf("</body>");
                var bodyXml = fb2Xml.Substring(bodyStart, bodyEnd - bodyStart + 7);

                var debugIndex = 762800;
                if (bodyXml.Length >= debugIndex + 25)
                {
                    var debugString = bodyXml.Substring(debugIndex - 25, 50);
                    System.Diagnostics.Debug.WriteLine(fileName + " : " + debugString);
                }

                var bodyDoc = new XmlDocument();
                bodyDoc.LoadXml(bodyXml);
                var bodyRoot = bodyDoc.DocumentElement;
                var commentIds = new List<string>();
                foreach (XmlNode node in bodyRoot.ChildNodes)
                {
                    HandleFb2Node(node, jsonFileData, imageIds, commentIds, fileName, 0);
                }

                // 2. save images
                var fb2Doc = new XmlDocument();
                fb2Doc.LoadXml(fb2Xml);
                foreach (XmlNode node in fb2Doc.DocumentElement.ChildNodes)
                {
                    if (node.Name == "binary")
                    {
                        var imageId = $"{fileName}{node.Attributes["id"].Value}";
                        var imageIndex = imageIds.IndexOf(imageId);
                        if (imageIndex != -1)
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
                }

                // 3. get comment texts
                var comments = new Dictionary<string, JsonCommentData>();
                var parComments = jsonFileData.Pars.SelectMany(p => p.Comments ?? new List<JsonCommentData>()).ToList();
                foreach (var cd in parComments)
                {
                    if (comments.ContainsKey(cd.CommentId)) continue;
                    comments.Add(cd.CommentId, cd);
                }
                foreach (var commentId in comments.Keys)
                {
                    var commentNode = XmlToJsonUtil.GetCommentNode(fb2Xml, commentId);

                    if (commentNode == null)
                    {
                        Console.WriteLine($"Comment '{commentId}' xml node was not found");
                        continue;
                    }

                    comments[commentId].Text = commentNode.InnerText;
                }

                // 4. keep headings
                if (keepHeadings && existingJsonDatas.ContainsKey(jsonFile))
                {
                    var existingJsonData = existingJsonDatas[jsonFile];
                    foreach (var jsonHeading in jsonFileData.Headings)
                    {
                        var existingJsonHeading = existingJsonData.Headings.FirstOrDefault(h => h.Text == jsonHeading.Text);
                        if (existingJsonHeading == null)
                        {
                            Console.WriteLine($"Existing heading not found for '{Path.GetFileName(jsonFile)}':{Environment.NewLine} {jsonHeading.Text}");
                            continue;
                        }

                        jsonHeading.Level = existingJsonHeading.Level;
                    }
                }

                File.WriteAllText(jsonFile, JsonConvert.SerializeObject(jsonFileData, Formatting.Indented));
            }
        }

        private void HandleFb2Node(XmlNode node, JsonFileData jsonFileData, List<string> imageIds, List<string> commentIds, string fileName, byte sectionLevel)
        {
            var sectionNodeId = node.Attributes?["id"]?.Value;

            if (sectionNodeId != null && commentIds.Contains(sectionNodeId)) return;

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
                        Level = sectionLevel,
                        Text = titleParagraph.Text
                    });
                    break;
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
                case "section":
                    var childNodes = node.ChildNodes;
                    foreach (XmlNode childNode in childNodes)
                    {
                        HandleFb2Node(childNode, jsonFileData, imageIds, commentIds, fileName, (byte)(sectionLevel + 1));
                    }
                    break;
            }
        }
    }
}