using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using LeninSearch.Standard.Core;
using LeninSearch.Standard.Core.Corpus;
using LeninSearch.Standard.Core.Optimized;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace LeninSearch.Api.Services
{
    public class PrecachedLsiProvider : ILsiProvider
    {
        private readonly int[] _corpusVersions;
        private string Key(string filePath, int corpusVersion) => $"{filePath}_{corpusVersion}";

        private readonly Dictionary<string, LsIndexData> _lsIndexData = new Dictionary<string, LsIndexData>();

        private readonly Dictionary<int, Corpus> _mains = new Dictionary<int, Corpus>();

        private readonly Dictionary<int, string[]> _dictionaries = new Dictionary<int, string[]>();

        public PrecachedLsiProvider(params int[] corpusVersions)
        {
            _corpusVersions = corpusVersions;
        }

        public PrecachedLsiProvider Load()
        {
            foreach (var corpusVersion in _corpusVersions)
            {
                var executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var folder = Path.Combine(executingDirectory, "corpus", $"v{corpusVersion}");
                var mainJson = File.ReadAllText(Path.Combine(folder, "main.json"));
                var main = JsonConvert.DeserializeObject<Corpus>(mainJson);
                _mains.Add(corpusVersion, main);

                var dictionary = File.ReadAllText(Path.Combine(folder, "corpus.dic")).Split('\n', StringSplitOptions.RemoveEmptyEntries);
                _dictionaries.Add(corpusVersion, dictionary);

                foreach (var ci in main.Items)
                {
                    foreach (var cfi in ci.Files)
                    {
                        var lsiBytes = File.ReadAllBytes(Path.Combine(folder, cfi.Path));
                        var lsi = LsIndexUtil.FromLsIndexBytes(lsiBytes);
                        var key = Key(cfi.Path, corpusVersion);
                        _lsIndexData.Add(key, lsi);
                    }
                }
            }

            return this;
        }

        public LsIndexData GetLsiData(int corpusVersion, string filePath)
        {
            var key = Key(filePath, corpusVersion);

            return _lsIndexData[key];
        }

        public string[] GetDictionary(int corpusVersion)
        {
            return _dictionaries[corpusVersion];
        }

        public Corpus GetCorpus(int corpusVersion)
        {
            return _mains[corpusVersion];
        }
    }
}