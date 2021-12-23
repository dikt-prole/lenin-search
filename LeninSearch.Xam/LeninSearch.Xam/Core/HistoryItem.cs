using System;

namespace LeninSearch.Xam.Core
{
    public class HistoryItem
    {
        public DateTime QueryDateUtc { get; set; }
        public string Query { get; set; }
        public string CorpusId { get; set; }
        public string CorpusName { get; set; }
    }
}