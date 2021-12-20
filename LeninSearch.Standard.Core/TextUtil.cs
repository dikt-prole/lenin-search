using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LeninSearch.Standard.Core
{
    public class TextUtil
    {
        private static readonly Regex CleanupRegex = new Regex("[^а-яa-z -]");
        public static List<string> GetIndexWords(string text)
        {
            if (string.IsNullOrEmpty(text)) return new List<string>();

            text = text.ToLower();

            text = text.Replace(" - ", " ");

            text = CleanupRegex.Replace(text, "");

            return text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
        }

        public static List<string> GetOrderedWords(string text)
        {
            if (string.IsNullOrEmpty(text)) return new List<string>();

            var words = new List<string>();

            var lst = SymbolType.Other;
            var builder = new StringBuilder();
            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];
                var cst = char.IsLetter(c)
                    ? SymbolType.Letter
                    : char.IsDigit(c)
                        ? SymbolType.Digit
                        : SymbolType.Other;

                if (lst == SymbolType.Letter)
                {
                    switch (cst)
                    {
                        case SymbolType.Digit:
                            words.Add(builder.ToString());
                            builder = new StringBuilder();
                            builder.Append(c);
                            break;
                        case SymbolType.Other:
                            words.Add(builder.ToString());
                            words.Add(new string(c, 1));
                            builder = new StringBuilder();
                            break;
                        default:
                            builder.Append(c);
                            break;
                    }
                }

                else if (lst == SymbolType.Digit)
                {
                    switch (cst)
                    {
                        case SymbolType.Letter:
                            words.Add(builder.ToString());
                            builder = new StringBuilder();
                            builder.Append(c);
                            break;
                        case SymbolType.Other:
                            words.Add(builder.ToString());
                            words.Add(new string(c, 1));
                            builder = new StringBuilder();
                            break;
                        default:
                            builder.Append(c);
                            break;
                    }
                }

                else
                {
                    switch (cst)
                    {
                        case SymbolType.Letter:
                            builder.Append(c);
                            break;
                        case SymbolType.Digit:
                            builder.Append(c);
                            break;
                        default:
                            words.Add(new string(c, 1));
                            break;
                    }
                }

                lst = cst;
            }

            if (builder.Length != 0)
            {
                words.Add(builder.ToString());
            }

            return words.Where(w => w != "" && w != " " && w != "\r" && w != "\n").ToList();
        }
        public static string GetParagraph(List<string> words)
        {
            if (words == null || words.Count == 0) return null;

            int citationCount = 0;
            var builder = new StringBuilder();
            builder.Append(words[0]);

            var prevWord = words[0];
            for (var i = 1; i < words.Count; i++)
            {
                var curWord = words[i];

                if (NeedAppendSpace(prevWord, curWord, ref citationCount)) builder.Append(' ');

                builder.Append(curWord);

                prevWord = words[i];
            }

            return builder.ToString();
        }

        private static readonly char[] OpenBrackets = { '(', '[', '{', '«' };
        private static readonly char[] ClosingBrackets = { ')', ']', '}', '»' };
        private static readonly char[] Punctuation = { '.', '!', '?', ',', ';', ':' };
        private static bool NeedAppendSpace(string prevWord, string curWord, ref int citationCount)
        {
            var prevChar = prevWord.Last();
            var curChar = curWord.First();

            if (OpenBrackets.Contains(prevChar)) return false;

            if (ClosingBrackets.Contains(prevChar)) return true;

            if (OpenBrackets.Contains(curChar)) return true;

            if (ClosingBrackets.Contains(curChar)) return false;

            if (char.IsLetter(curChar)) return true;

            if (char.IsDigit(curChar) && (prevChar == ',' || prevChar == '.')) return false;

            if (char.IsDigit(curChar)) return true;

            if (Punctuation.Contains(curChar)) return false;

            if (curChar == '\'' || curChar == '"')
            {
                if (citationCount == 0)
                {
                    citationCount = citationCount + 1;
                    return true;
                }

                citationCount = citationCount - 1;
                return false;
            }

            return true;
        }
        private enum SymbolType { Letter = 0, Digit = 1, Other = 2 }
    }
}