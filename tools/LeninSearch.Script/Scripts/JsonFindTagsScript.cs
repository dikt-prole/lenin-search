using System;
using System.IO;
using LeninSearch.Standard.Core.Corpus.Json;
using Newtonsoft.Json;

namespace LeninSearch.Script.Scripts
{
    public class JsonFindTagsScript : IScript
    {
        public string Id => "find-tag";
        public string Arguments => "(json folder)";
        public void Execute(params string[] input)
        {
            var jsonFolder = input[0];
            var jsonFiles = Directory.GetFiles(jsonFolder, "*.json");

            foreach (var jsonFile in jsonFiles)
            {
                var fileData = JsonConvert.DeserializeObject<JsonFileData>(File.ReadAllText(jsonFile));
                foreach (var jsonParagraph in fileData.Pars)
                {
                    if (jsonParagraph.Text.Contains("<"))
                    {
                        Console.WriteLine(jsonFile);
                        Console.WriteLine(jsonParagraph.Text);
                        Console.WriteLine();
                    }
                }
            }
        }
    }
}