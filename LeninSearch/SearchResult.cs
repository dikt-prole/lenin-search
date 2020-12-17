using System;
using System.Collections.Generic;
using LeninSearch.Core;
using LeninSearch.Core.Oprimized;

namespace LeninSearch
{
    public class SearchResult
    {
        public Guid Id { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public List<OptimizedParagraph> OptimizedParagraphs { get; set; }
        public SearchOptions SearchOptions { get; set; }
        public OptimizedFileData OptimizedFileData { get; set; }
        private string FileNameWithoutExtension => FileUtil.GetFileNameWithoutExtension(SearchOptions.File);

        public override string ToString()
        {
            if (Success)
            {
                return $"{FileNameWithoutExtension} ({OptimizedParagraphs?.Count ?? 0})";
            }

            return $"{FileNameWithoutExtension} - ошибка";
        }
    }
}