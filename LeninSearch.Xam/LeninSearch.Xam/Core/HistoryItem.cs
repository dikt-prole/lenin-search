using System;

namespace LeninSearch.Xam.Core
{
    public class HistoryItem
    {
        public string Corpus { get; set; }
        public DateTime QueryDateUtc { get; set; }
        public string Query { get; set; }
    }
}