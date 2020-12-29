using System;
using System.Collections.Generic;
using System.Linq;

namespace LeninSearch.Core
{
    public class SearchOptions
    {
        public SearchOptions() {}

        public SearchOptions(string query, string file, Dictionary<string, uint> reversedDictionary)
        {
            File = file;

            var querySplit = query.Split('+');
            MainQuery = querySplit[0].Trim(' ').ToLower();


            AdditionalQuery = querySplit.Length > 1 ? querySplit.Last().Trim(' ').ToLower() : null;

            var mainWords = TextUtil.GetOrderedWords(MainQuery);
            var additionalWords = TextUtil.GetOrderedWords(AdditionalQuery);

            WordIndexes = new List<List<uint>>();
            foreach (var w in mainWords)
            {
                var wIndexes = new List<uint>();
                WordIndexes.Add(wIndexes);

                foreach (var rdKey in reversedDictionary.Keys)
                {
                    if (rdKey.ToLower() == w)
                    {
                        wIndexes.Add(reversedDictionary[rdKey]);
                    }
                }

                if (wIndexes.Count == 0)
                {
                    WordIndexes = new List<List<uint>>();
                    return;
                }
            }

            foreach (var w in additionalWords)
            {
                var wIndexes = new List<uint>();
                WordIndexes.Add(wIndexes);

                foreach (var rdKey in reversedDictionary.Keys)
                {
                    if (rdKey.ToLower().StartsWith(w))
                    {
                        wIndexes.Add(reversedDictionary[rdKey]);
                    }
                }

                if (wIndexes.Count == 0)
                {
                    WordIndexes = new List<List<uint>>();
                    return;
                }
            }
        }

        public string MainQuery { get; set; }
        public string AdditionalQuery { get; set; }
        public List<List<uint>> WordIndexes { get; set; }
        public string File { get; set; }

        public SearchOptions Copy()
        {
            return new SearchOptions
            {
                MainQuery = MainQuery,
                AdditionalQuery = AdditionalQuery,
                WordIndexes = WordIndexes,
                File = File,
            };
        }
    }
}