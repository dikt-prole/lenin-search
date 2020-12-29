using System;
using System.Collections.Generic;
using LeninSearch.Core.Oprimized;

namespace LeninSearch.Core
{
    public class SearchResult
    {
        public Guid Id { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public List<OptimizedParagraph> OptimizedParagraphs { get; set; }
        public SearchOptions SearchOptions { get; set; }
        public OptimizedFileData OptimizedFileData { get; set; }

        public override string ToString()
        {
            if (Success)
            {
                return $"{SearchOptions.File} ({OptimizedParagraphs?.Count ?? 0})";
            }

            return $"{SearchOptions.File} - ошибка";
        }
    }
}