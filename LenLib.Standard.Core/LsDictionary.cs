using System.Collections.Generic;

namespace LenLib.Standard.Core
{
    public class LsDictionary
    {
        private readonly Dictionary<string, uint> _reversed;
        public string[] Words { get; }
        public uint this[string word] => _reversed[word];
        public string this[uint index] => Words[index];
        public LsDictionary(string[] words)
        {
            Words = words;
            _reversed = new Dictionary<string, uint>();
            for (uint i = 0; i < words.Length; i++)
            {
                _reversed.Add(words[i], i);
            }
        }
    }
}