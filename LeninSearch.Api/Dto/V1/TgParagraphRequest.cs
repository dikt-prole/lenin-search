namespace LeninSearch.Api.Dto.V1
{
    public class TgParagraphRequest
    {
        public string CorpusId { get; set; }
        public string Path { get; set; }
        public ushort ParagraphIndex { get; set; }
    }
}