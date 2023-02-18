using LeninSearch.Standard.Core.Search;

namespace LeninSearch.Api.Dto.V1
{
    public class TgSearchUnitResponse
    {
        public string Path { get; set; }
        public ushort ParagraphIndex { get; set; }
        public string Preview { get; set; }
        public string Title { get; set; }
        public int Priority { get; set; }
    }
}