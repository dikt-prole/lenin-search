using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LeninSearch.Core
{
    public class TextUtil
    {
        public static readonly Regex LettersOnlyRegex = new Regex("[^А-Яа-я]");

        public static string NormalizeText(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            text = string.Join(' ', text.Split(' ').Select(NormalizeWord));

            const string replacer = "965db4ba";
            text = text.Replace(" - ", replacer);
            text = text.Replace("- ", "");
            text = text.Replace(replacer, " - ");

            return text;
        }

        private static string NormalizeWord(string word)
        {
            if (word.All(c => !char.IsLetter(c))) return word;

            if (word.EndsWith(','))
            {
                return word.Trim('1', '2', '3', '4', '5', '6', '7', '8', '9', '0', ',') + ',';
            }

            return word.Trim('1', '2', '3', '4', '5', '6', '7', '8', '9', '0');
        }

        private static readonly Regex CleanupRegex = new Regex("[^а-яa-z -]");
        public static List<string> GetIndexWords(string text)
        {
            if (string.IsNullOrEmpty(text)) return new List<string>();

            text = text.ToLower();

            text = text.Replace(" - ", " ");

            text = CleanupRegex.Replace(text, "");

            return text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
        }

        private static readonly char[] NumericSymbols = { '-', '=', '+', '~', ',', '.', '/', ' ' };
        private static readonly Regex NumericRegex = new Regex($"[^0-9{new string(NumericSymbols)}]");
        public static List<int> GetNumbers(string text)
        {
            if (string.IsNullOrEmpty(text)) return new List<int>();

            text = new string(text.Select(c => char.IsNumber(c) ? c : ' ').ToArray());

            var split = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var numbers = new List<int>();
            foreach (var s in split)
            {
                if (int.TryParse(s, out var number))
                {
                    numbers.Add(number);
                }
            }

            return numbers;
        }

        private static readonly Regex AllRussianUpperRegex = new Regex("[^А-Яа-я]");
        public static bool AllRussianUpper(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;

            text = AllRussianUpperRegex.Replace(text, "");

            if (string.IsNullOrWhiteSpace(text)) return false;

            return text.All(char.IsUpper);
        }
    }
}