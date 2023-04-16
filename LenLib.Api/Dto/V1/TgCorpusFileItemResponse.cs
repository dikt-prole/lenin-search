using LenLib.Standard.Core.Corpus;

namespace LenLib.Api.Dto.V1
{
    public class TgCorpusFileItemResponse
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public static TgCorpusFileItemResponse FromCorpusFileItem(CorpusFileItem corpusFileItem)
        {
            return new TgCorpusFileItemResponse
            {
                Name = corpusFileItem.Name,
                Path = corpusFileItem.Path
            };
        }
    }
}