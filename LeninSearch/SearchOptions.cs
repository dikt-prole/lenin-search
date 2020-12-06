namespace LeninSearch
{
    public class SearchOptions
    {
        public string SearchText { get; set; }
        public string AdditionalText { get; set; }
        public string Corpus { get; set; }
        public string File { get; set; }

        public SearchOptions Copy()
        {
            return new SearchOptions
            {
                SearchText = SearchText,
                AdditionalText = AdditionalText,
                Corpus = Corpus,
                File = File,
            };
        }
    }
}