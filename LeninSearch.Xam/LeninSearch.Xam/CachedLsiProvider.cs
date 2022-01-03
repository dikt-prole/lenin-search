using System;
using System.IO;
using System.Linq;
using LeninSearch.Standard.Core;
using LeninSearch.Standard.Core.Corpus;
using LeninSearch.Standard.Core.Corpus.Json;
using LeninSearch.Standard.Core.Corpus.Lsi;
using LeninSearch.Standard.Core.LsiUtil;
using LeninSearch.Standard.Core.Search;
using Newtonsoft.Json;

namespace LeninSearch.Xam
{
    public class CachedLsiProvider : ILsiProvider
    {
        private LsDictionary _currentDictionary;

        private string _currentDictionaryId;

        private LsiData _currentLsi;

        private string _currentLsiKey;
        private string GetLsiKey(string corpusId, string file) => $"{corpusId}-{file}";

        public LsiData GetLsiData(string corpusId, string file)
        {
            var lsiKey = GetLsiKey(corpusId, file);

            if (_currentLsiKey == lsiKey && _currentLsi != null)
            {
                return _currentLsi;
            }

            _currentLsiKey = lsiKey;

            var lsiBytes = File.ReadAllBytes(Path.Combine(Settings.CorpusRoot, corpusId, file));

            var lsiUtil = LsiUtilLocator.GetLsiUtil(lsiBytes[0]);

            _currentLsi = lsiUtil.FromLsIndexBytes(lsiBytes);

            return _currentLsi;
        }

        public LsDictionary GetDictionary(string corpusId)
        {
            if (_currentDictionaryId == corpusId && _currentDictionary != null)
            {
                return _currentDictionary;
            }

            _currentDictionaryId = corpusId;

            var dictionaryPath = Path.Combine(Settings.CorpusRoot, corpusId, "corpus.dic");

            _currentDictionary = new LsDictionary(File.ReadAllLines(dictionaryPath).Where(s => !string.IsNullOrEmpty(s)).ToArray());

            return _currentDictionary;
        }

        public CorpusItem GetCorpusItem(string corpusId)
        {
            var corpusItemPath = Path.Combine(Settings.CorpusRoot, corpusId, "corpus.json");

            return JsonConvert.DeserializeObject<CorpusItem>(File.ReadAllText(corpusItemPath));
        }

        public void CleanCache()
        {
            _currentDictionary = null;
            _currentDictionaryId = null;
            _currentLsiKey = null;
            _currentLsi = null;
        }
    }
}