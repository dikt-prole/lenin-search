using System;
using System.Collections.Generic;
using System.Linq;

namespace LenLib.Standard.Core.Search
{
    public class SearchQueryCleaner
    {
        public string Clean(string searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                return searchQuery;
            }

            searchQuery = new string(searchQuery.Where(IsAllowedSymbol).ToArray());

            searchQuery = AllowMax(searchQuery, '+', 1, ' ');

            searchQuery = searchQuery.Replace("*", "* ").Replace("+", " + ");

            var spaceSplit = searchQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            spaceSplit = spaceSplit.Where(s => s != "*").ToArray();

            return string.Join(' ', spaceSplit).TrimEnd(' ', '+');
        }

        private bool IsAllowedSymbol(char c)
        {
            return char.IsLetter(c) || char.IsDigit(c) || c == ' ' || c == '*' || c == '+';
        }

        private string AllowMax(string text, char targetChar, ushort max, char replaceChar)
        {
            var chars = new List<char>();
            var count = 0;
            foreach (var @char in text)
            {
                if (@char == targetChar)
                {
                    count++;
                    chars.Add(count <= max ? @char : replaceChar);
                }
                else
                {
                    chars.Add(@char);
                }
            }

            return new string(chars.ToArray());
        }
    }
}