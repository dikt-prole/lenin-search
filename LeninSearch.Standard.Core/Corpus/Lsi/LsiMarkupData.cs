using LeninSearch.Standard.Core.Corpus.Json;

namespace LeninSearch.Standard.Core.Corpus.Lsi
{
    public class LsiMarkupData
    {
        public ushort WordPosition { get; set; }
        public ushort WordLength { get; set; }
        public MarkupType MarkupType { get; set; }
    }
}