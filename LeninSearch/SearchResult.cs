using System;
using System.Collections.Generic;
using LeninSearch.Core;

namespace LeninSearch
{
    public class SearchResult
    {
        public Guid Id { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public List<int> ParagraphIndexes { get; set; }
        public SearchOptions SearchOptions { get; set; }
        public FileData FileData { get; set; }
        private string FileNameWithoutExtension => FileUtil.GetFileNameWithoutExtension(SearchOptions.File);

        public override string ToString()
        {
            if (Success)
            {
                return $"{FileNameWithoutExtension} ({ParagraphIndexes?.Count ?? 0})";
            }

            return $"{FileNameWithoutExtension} - ошибка";
        }
    }
}