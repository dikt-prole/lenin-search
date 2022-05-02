using System;
using System.IO;
using LeninSearch.Standard.Core.Corpus.Json;
using Newtonsoft.Json;

namespace LeninSearch.Script.Scripts
{
    public class LsaSplitScript : IScript
    {
        public string Id => "lsa-split-script";
        public string Arguments => "json-folder, split-folder";
        public void Execute(params string[] input)
        {
            var jsonFolder = input[0];
            var splitFolder = input[1];

            var jsonFiles = Directory.GetFiles(jsonFolder, "*.json");
            foreach (var jsonFile in jsonFiles)
            {
                var jsonData = JsonConvert.DeserializeObject<JsonFileData>(File.ReadAllText(jsonFile));
                for (var i = 0; i < jsonData.Pars.Count; i++)
                {
                    var parText = jsonData.Pars[i].Text;

                    if (string.IsNullOrEmpty(parText)) continue;

                    var parFile = Path.Combine(splitFolder, $"{Path.GetFileNameWithoutExtension(jsonFile)}_{i}.txt");

                    File.WriteAllText(parFile, parText);
                }
            }
        }
    }
}
