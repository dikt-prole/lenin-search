using System;
using System.Collections.Generic;
using System.IO;
using LeninSearch.Standard.Core;
using LeninSearch.Standard.Core.Corpus;
using LeninSearch.Standard.Core.Optimized;
using LeninSearch.Standard.Core.Search;
using Newtonsoft.Json;

namespace LeninSearch.Xam
{
    public class CachedLsiProvider : ILsiProvider
    {
        private LsDictionary _dictionary;

        private readonly Dictionary<int, Corpus> _corpuses = new Dictionary<int, Corpus>();

        private string _currentLsiPath;

        private LsIndexData _currentLsi;

        public LsIndexData GetLsiData(int corpusVersion, string filePath)
        {
            if (_currentLsiPath == filePath && _currentLsi != null)
            {
                return _currentLsi;
            }

            _currentLsiPath = filePath;

            var lsiBytes = File.ReadAllBytes(Path.Combine(Settings.CorpusFolder, filePath));

            _currentLsi = LsIndexUtil.FromLsIndexBytes(lsiBytes);

            return _currentLsi;
        }

        public LsDictionary GetDictionary(int corpusVersion)
        {
            if (_dictionary == null)
            {
                var corpusDicPath = Path.Combine(Settings.CorpusFolder, "main.dic");
                var text = File.ReadAllText(corpusDicPath);
                var words = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                _dictionary = new LsDictionary(words);
            }

            return _dictionary;
        }

        public Corpus GetCorpus(int corpusVersion)
        {
            if (!_corpuses.ContainsKey(corpusVersion))
            {
                var mainJson = File.ReadAllText(Path.Combine(Settings.CorpusFolder, "main.json"));
                var corpus = JsonConvert.DeserializeObject<Corpus>(mainJson);
                _corpuses.Add(corpusVersion, corpus);
            }

            return _corpuses[corpusVersion];
        }

        public void CleanCache()
        {
            _corpuses.Clear();
            _dictionary = null;
            _currentLsiPath = null;
            _currentLsi = null;
        }
    }
}