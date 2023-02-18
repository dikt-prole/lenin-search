using System.Collections.Generic;
using System.Linq;
using LeninSearch.Standard.Core.Corpus;

namespace LeninSearch.Api.Dto.V1
{
    public class TgCorpusItemResponse
    {
        public string Id { get; set; }
        public string Series { get; set; }
        public int CorpusVersion { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TgCorpusFileItemResponse[] Files { get; set; }

        public static TgCorpusItemResponse FromCorpusItem(CorpusItem corpusItem)
        {
            return new TgCorpusItemResponse
            {
                Id = corpusItem.Id,
                Name = corpusItem.Name,
                Series = corpusItem.Series,
                CorpusVersion = corpusItem.CorpusVersion,
                Description = corpusItem.Description,
                Files = corpusItem.Files
                    .Where(cfi => cfi.Path.EndsWith(".lsi"))
                    .Select(TgCorpusFileItemResponse.FromCorpusFileItem).ToArray()
            };
        }
    }
}