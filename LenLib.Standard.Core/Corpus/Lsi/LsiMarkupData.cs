using LenLib.Standard.Core.Corpus.Json;

namespace LenLib.Standard.Core.Corpus.Lsi
{
    public class LsiMarkupData
    {
        public ushort WordPosition { get; set; }
        public ushort WordLength { get; set; }
        public MarkupType MarkupType { get; set; }
    }
}