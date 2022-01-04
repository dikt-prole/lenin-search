using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeninSearch.Standard.Core;
using LeninSearch.Standard.Core.Corpus;
using LeninSearch.Standard.Core.Corpus.Json;
using LeninSearch.Standard.Core.LsiUtil;
using Newtonsoft.Json;
using Encoder = System.Drawing.Imaging.Encoder;

namespace LeninSearch.Script.Scripts
{
    public class JsonToLsiScript : IScript
    {
        private static readonly Random Random = new Random();
        public string Id => "json-to-lsi";
        public string Arguments => "json_folder, lsi_folder, jpeg_quality?";

        private ILsiUtil _lsiUtil;

        public JsonToLsiScript()
        {
            _lsiUtil = new V2LsiUtil();
        }

        public void Execute(params string[] input)
        {
            var jsonFolder = input[0];
            var lsiFolder = input[1];
            var jpegQuality = input.Length == 3 ? long.Parse(input[2]) : -1;

            foreach (var file in Directory.GetFiles(lsiFolder)) File.Delete(file);

            var dicFile = Path.Combine(lsiFolder, "corpus.dic");
            Console.WriteLine($"Json Folder: {jsonFolder}");
            Console.WriteLine($"Lsi Folder: {lsiFolder}");
            Console.WriteLine($"Dic File: {dicFile}");

            var jsonFiles = Directory.GetFiles(jsonFolder, "*.json");
            var globalWords = new HashSet<string>();

            Console.WriteLine("Constructing .dic file");
            foreach (var jsonFile in jsonFiles)
            {
                var fileData = JsonConvert.DeserializeObject<JsonFileData>(File.ReadAllText(jsonFile));
                foreach (var paragraph in fileData.Pars)
                {
                    var words = TextUtil.GetOrderedWords(paragraph.Text);
                    foreach (var word in words)
                    {
                        if (!globalWords.Contains(word)) globalWords.Add(word);
                    }

                    foreach (var comment in paragraph.Comments ?? new List<JsonCommentData>())
                    {
                        var commentWords = TextUtil.GetOrderedWords(comment.Text);
                        foreach (var word in commentWords)
                        {
                            if (!globalWords.Contains(word)) globalWords.Add(word);
                        }
                    }
                }

                if (fileData.Headings != null)
                {
                    foreach (var heading in fileData.Headings)
                    {
                        var words = TextUtil.GetOrderedWords(heading.Text);
                        foreach (var word in words)
                        {
                            if (!globalWords.Contains(word)) globalWords.Add(word);
                        }
                    }
                }
            }

            Console.WriteLine($"Global words count: {globalWords.Count}");
            File.AppendAllLines(dicFile, globalWords, Encoding.UTF8);

            var globalWordsArray = File.ReadAllLines(dicFile).Where(s => !string.IsNullOrEmpty(s)).ToArray();
            var globalWordsDictionary = new Dictionary<string, uint>();
            for (var i = 0; i < globalWordsArray.Length; i++)
            {
                globalWordsDictionary.Add(globalWordsArray[i], (uint)i);
            }

            var tasks = jsonFiles.Select(jf => Task.Run(() => WriteLsiFile(jf, lsiFolder, globalWordsDictionary))).ToList();

            Task.WhenAll(tasks).Wait();

            Console.WriteLine("Copying images");
            var jsonImages = Directory.GetFiles(jsonFolder, "*.jpeg");
            foreach (var jsonImageFile in jsonImages)
            {
                using (var jsonImage = Image.FromFile(jsonImageFile))
                {
                    var lsiImageFile = Path.Combine(lsiFolder, Path.GetFileName(jsonImageFile));
                    if (jpegQuality > 0)
                    {
                        var encoderParams = new EncoderParameters(1);
                        var qualityParam = new EncoderParameter(Encoder.Quality, jpegQuality);
                        var jpegEncoder = ImageCodecInfo.GetImageEncoders().First(e => e.MimeType == "image/jpeg");
                        encoderParams.Param[0] = qualityParam;
                        jsonImage.Save(lsiImageFile, jpegEncoder, encoderParams);
                    }
                    else
                    {
                        jsonImage.Save(lsiImageFile, ImageFormat.Jpeg);
                    }
                }
            }

            Console.WriteLine("Construct corpus.json");
            var corpusFileItems = new List<CorpusFileItem>();

            corpusFileItems.Add(new CorpusFileItem
            {
                Name = "corpus.dic",
                Path = "corpus.dic",
                Size = File.ReadAllBytes(dicFile).Length
            });

            corpusFileItems.Add(new CorpusFileItem
            {
                Name = "icon.png",
                Path = "icon.png",
                Size = 15 * 1024
            });

            var lsiFiles = Directory.GetFiles(lsiFolder, "*.lsi");
            corpusFileItems.AddRange(lsiFiles.Select(f => new CorpusFileItem
            {
                Path = Path.GetFileName(f),
                Name = Path.GetFileName(f),
                Size = File.ReadAllBytes(f).Length
            }));

            var lsiImages = Directory.GetFiles(lsiFolder, "*.jpeg")
                .OrderByDescending(f => int.Parse(new string(f.Where(char.IsNumber).ToArray()))).ToList();
            corpusFileItems.AddRange(lsiImages.Select(f => new CorpusFileItem
            {
                Path = Path.GetFileName(f),
                Name = Path.GetFileName(f),
                Size = File.ReadAllBytes(f).Length
            }));

            corpusFileItems.Add(new CorpusFileItem
            {
                Name = "corpus.json",
                Path = "corpus.json",
                Size = 8 * 1024
            });

            var lsiFolderName = Path.GetDirectoryName(lsiFiles[0]).Split('\\').Last();
            var lsiFolderNameSplit = lsiFolderName.Split('-');
            var corpusItem = new CorpusItem
            {
                Id = lsiFolderName,
                Description = lsiFolderName,
                Name = lsiFolderName,
                Series = string.Join("-", lsiFolderNameSplit.Take(lsiFolderNameSplit.Length - 1)),
                CorpusVersion = int.Parse(new string(lsiFolderNameSplit.Last().Where(char.IsNumber).ToArray())),
                Files = corpusFileItems,
                LsiVersion = 2
            };

            var corpusJsonFile = Path.Combine(lsiFolder, "corpus.json");
            File.WriteAllText(corpusJsonFile, JsonConvert.SerializeObject(corpusItem, Formatting.Indented));
            var iconBytes = File.ReadAllBytes($"D:\\Repo\\lenin-search\\corpus\\icons\\{corpusItem.Series}.png");
            var iconFile = Path.Combine(lsiFolder, "icon.png");
            File.WriteAllBytes(iconFile, iconBytes);

            Console.WriteLine();
            Console.WriteLine($"Corpus size with images: {1.0 * corpusFileItems.Sum(cfi => cfi.Size) / 1024 / 1024:F2}mb");
            var nonImageFiles = corpusFileItems.Where(cfi => !cfi.Path.EndsWith("jpeg")).ToList();
            Console.WriteLine($"Corpus size without images: {1.0 * nonImageFiles.Sum(cfi => cfi.Size) / 1024 / 1024:F2}mb");
            Console.WriteLine();

            foreach (var lsiFile in lsiFiles)
            {
                var lsiFileName = Path.GetFileName(lsiFile);
                Console.WriteLine($"Verifying {lsiFileName}");
                var lsiData = _lsiUtil.FromLsIndexBytes(File.ReadAllBytes(lsiFile));
                var jsonFile = Path.Combine(jsonFolder, Path.GetFileName(lsiFile).Replace(".lsi", ".json"));
                var jsonData = JsonConvert.DeserializeObject<JsonFileData>(File.ReadAllText(jsonFile));

                var lsiParagraphCount = lsiData.Paragraphs.Count;
                var jsonParagraphCount = jsonData.Pars.Count(p => !string.IsNullOrEmpty(p.Text));
                if (lsiParagraphCount != jsonParagraphCount)
                {
                    Console.WriteLine($"{lsiParagraphCount} vs {jsonParagraphCount} json paragraphs");
                }

                var lsiHeadingCount = lsiData.Headings.Count;
                var jsonHeadingCount = jsonData.Headings.Count;
                if (lsiHeadingCount != jsonHeadingCount)
                {
                    Console.WriteLine($"{lsiHeadingCount} vs {jsonHeadingCount} json headings");
                }

                var lsiImageCount = lsiData.Images.Count;
                var jsonImageCount = jsonData.Pars.Count(p => p.ImageIndex.HasValue);
                if (lsiImageCount != jsonImageCount)
                {
                    Console.WriteLine($"{lsiImageCount} vs {jsonImageCount} json images");
                }

                var lsiMarkupCount = lsiData.Markups.SelectMany(m => m.Value).Count();
                var jsonMarkupCount = jsonData.Pars.Where(p => p.Markups != null).SelectMany(p => p.Markups).Count();
                if (lsiMarkupCount != jsonMarkupCount)
                {
                    Console.WriteLine($"{lsiMarkupCount} vs {jsonMarkupCount} json images");
                }

                var lsiCommentCount = lsiData.Comments.SelectMany(c => c.Value).Count();
                var jsonCommentCount = jsonData.Pars.Where(p => p.Comments != null).SelectMany(p => p.Comments).Count();
                if (lsiMarkupCount != jsonMarkupCount)
                {
                    Console.WriteLine($"{lsiCommentCount} vs {jsonCommentCount} json images");
                }

                var paragraphKeys = lsiData.Paragraphs.Keys.ToArray();
                var randomParagraphIndex = paragraphKeys[Random.Next(0, paragraphKeys.Length)];
                var paragraph = lsiData.Paragraphs[randomParagraphIndex];
                var paragraphText = paragraph.GetText(globalWordsArray);
                Console.WriteLine($"Random text ({randomParagraphIndex}): {paragraphText}");
                Console.WriteLine();
            }
        }

        private void WriteLsiFile( string jsonFile, string lsiFolder, Dictionary<string, uint> globalWords)
        {
            try
            {
                var lsiFile = Path.Combine(lsiFolder, Path.GetFileName(jsonFile).Replace(".json", ".lsi"));
                Console.WriteLine($"Constructing bytes for '{lsiFile}'");
                var fileData = JsonConvert.DeserializeObject<JsonFileData>(File.ReadAllText(jsonFile));
                var lsiBytes = _lsiUtil.ToLsIndexBytes(fileData, globalWords);
                File.WriteAllBytes(lsiFile, lsiBytes);
            }
            catch (Exception exc)
            {
                Console.WriteLine($"{jsonFile}: {exc}");
            }
        }
    }
}