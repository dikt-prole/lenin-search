using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace LeninSearch.Xam.Controls
{
    public class LsKey
    {
        public string Key { get; }
        public string Paste { get; }

        private LsKey(string key) : this(key, key.ToLower()) { }

        private LsKey(string key, string paste)
        {
            Key = key;
            Paste = paste;
        }

        public static readonly LsKeyValues Values = new LsKeyValues();

        public class LsKeyValues
        {
            public static IEnumerable<string> AllKeys()
            {
                yield return "ТКСТ-1";
                yield return "ТКСТ-2";
                yield return "ТКСТ-3";
                yield return "ЗГЛВК";
                yield return "ПРКЛ";
                yield return "<";

                yield return "Й";
                yield return "Ц";
                yield return "У";
                yield return "К";
                yield return "Е";
                yield return "Н";
                yield return "Г";
                yield return "Ш";
                yield return "Щ";
                yield return "З";
                yield return "Х";

                yield return "Ф";
                yield return "Ы";
                yield return "В";
                yield return "А";
                yield return "П";
                yield return "Р";
                yield return "О";
                yield return "Л";
                yield return "Д";
                yield return "Ж";
                yield return "Э";

                yield return "Я";
                yield return "Ч";
                yield return "С";
                yield return "М";
                yield return "И";
                yield return "Т";
                yield return "Ь";
                yield return "Б";
                yield return "Ю";
                yield return "Ё";
                yield return "Ъ";

                yield return "0";
                yield return "1";
                yield return "2";
                yield return "3";
                yield return "4";
                yield return "5";
                yield return "6";
                yield return "7";
                yield return "8";
                yield return "9";
                yield return "SEARCH";
            }

            private readonly Dictionary<string, LsKey> _dictionary;

            public LsKeyValues()
            {
                _dictionary = new Dictionary<string, LsKey>();
                var allKeys = AllKeys().ToList();
                _dictionary.Add(allKeys[0], new LsKey(allKeys[0], Settings.Query.Txt1));
                _dictionary.Add(allKeys[1], new LsKey(allKeys[1], Settings.Query.Txt2));
                _dictionary.Add(allKeys[2], new LsKey(allKeys[2], Settings.Query.Txt3));
                _dictionary.Add(allKeys[3], new LsKey(allKeys[3], Settings.Query.Title1));
                _dictionary.Add(allKeys[4], new LsKey(allKeys[4]));
                _dictionary.Add(allKeys[5], new LsKey(allKeys[5]));
                foreach (var key in allKeys.Skip(6))
                {
                    _dictionary.Add(key, new LsKey(key));
                }
            }

            public LsKey this[string key] => _dictionary[key];
        }
    }
}