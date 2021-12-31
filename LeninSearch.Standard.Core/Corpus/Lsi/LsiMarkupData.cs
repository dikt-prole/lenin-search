using LeninSearch.Standard.Core.Corpus.Json;

namespace LeninSearch.Standard.Core.Corpus.Lsi
{
    public class LsiMarkupData
    {
        public ushort WordIndex { get; set; }
        public ushort Length { get; set; }
        public MarkupType MarkupType { get; set; }
    }
}