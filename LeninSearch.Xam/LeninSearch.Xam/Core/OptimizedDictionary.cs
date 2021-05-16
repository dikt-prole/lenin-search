using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LeninSearch.Xam.Core
{
    public class OptimizedDictionary
    {
        private string[] _direct;
        private Dictionary<string, uint> _reversed;
        private static OptimizedDictionary _instance;

        public static void Clear()
        {
            _instance = null;
        }

        public static OptimizedDictionary Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new OptimizedDictionary();
                    var dictionaryPath = $"{FileUtil.CorpusFolder}/main.dic";
                    _instance._direct = File.ReadAllLines(dictionaryPath).Where(s => s != "").ToArray();
                    _instance._reversed = new Dictionary<string, uint>();
                    for (uint i = 0; i < _instance._direct.Length; i++)
                    {
                        _instance._reversed.Add(_instance._direct[i], i);
                    }
                }

                return _instance;
            }
        }

        private OptimizedDictionary() { }
        public uint this[string word] => _reversed[word];
        public string this[uint index] => _direct[index];

        public string[] Words => _direct;

        public Dictionary<string, uint> Reversed => _reversed;
        public IEnumerable<uint> Indexes => _reversed.Values;
    }
}