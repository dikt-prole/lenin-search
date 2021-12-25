using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LeninSearch.Standard.Core;
using LeninSearch.Standard.Core.Corpus;
using Newtonsoft.Json;

namespace LeninSearch.Script.Scripts
{
    public class JsonToLsiScript : IScript
    {
        public string Id => "json-to-lsi";
        public void Execute(params string[] input)
        {
            var jsonFolder = input[0];
            var lsiFolder = input[1];
            var dicFile = Path.Combine(lsiFolder, "corpus.dic");
            if (File.Exists(dicFile)) File.Delete(dicFile);

            Console.WriteLine($"Json Folder: {jsonFolder}");
            Console.WriteLine($"Lsi Folder: {lsiFolder}");
            Console.WriteLine($"Dic File: {dicFile}");

            var jsonFiles = Directory.GetFiles(jsonFolder);
            var globalWords = new HashSet<string>();
            Console.WriteLine("Constructing .dic file");

            foreach (var jsonFile in jsonFiles)
            {
                var fileData = JsonConvert.DeserializeObject<FileData>(File.ReadAllText(jsonFile));
                foreach (var paragraph in fileData.Pars)
                {
                    var words = TextUtil.GetOrderedWords(paragraph.Text);
                    foreach (var word in words)
                    {
                        if (!globalWords.Contains(word)) globalWords.Add(word);
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

            var globalWordsList = File.ReadAllLines(dicFile).Where(s => !string.IsNullOrEmpty(s)).ToList();
            var globalWordsDictionary = new Dictionary<string, uint>();
            for (var i = 0; i < globalWordsList.Count; i++)
            {
                globalWordsDictionary.Add(globalWordsList[i], (uint)i);
            }

            var tasks = jsonFiles.Select(jf => Task.Run(()=> WriteLsiFile(jf, lsiFolder, globalWordsDictionary))).ToList();

            Task.WhenAll(tasks).Wait();
        }

        private static void WriteLsiFile(string jsonFile, string lsiFolder, Dictionary<string, uint> globalWords)
        {
            var lsiFile = Path.Combine(lsiFolder, Path.GetFileName(jsonFile).Replace(".json", ".lsi"));
            Console.WriteLine($"Constructing bytes for '{lsiFile}'");
            var fileData = JsonConvert.DeserializeObject<FileData>(File.ReadAllText(jsonFile));
            var lsiBytes = LsIndexUtil.ToLsIndexBytes(fileData, globalWords);
            File.WriteAllBytes(lsiFile, lsiBytes);
        }
    }
}