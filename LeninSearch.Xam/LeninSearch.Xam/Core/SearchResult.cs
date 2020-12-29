using System;
using System.Collections.Generic;
using LeninSearch.Xam.Core.Oprimized;

namespace LeninSearch.Xam.Core
{
    public class SearchResult
    {
        public Guid Id { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public List<OptimizedParagraph> OptimizedParagraphs { get; set; }
        public SearchOptions SearchOptions { get; set; }
        public OptimizedFileData OptimizedFileData { get; set; }
    }
}