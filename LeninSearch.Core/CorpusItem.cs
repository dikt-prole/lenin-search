namespace LeninSearch.Core
{
    public class CorpusItem
    {
        public string File { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public bool Selected { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Count})";
        }
    }
}