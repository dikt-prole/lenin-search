namespace LeninSearch.Standard.Core.Api
{
    public class CorpusSearchRequest
    {
        public int CorpusVersion { get; set; }
        public string CorpusName { get; set; }
        public string Query { get; set; }
    }
}