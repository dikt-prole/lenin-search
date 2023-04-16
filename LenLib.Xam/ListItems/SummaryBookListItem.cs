using System.Collections.Generic;
using LenLib.Standard.Core.Corpus;

namespace LenLib.Xam.ListItems
{
    public class SummaryBookListItem
    {
        public string CorpusId { get; set; }
        public string File { get; set; }
        public string Title { get; set; }
        public override string ToString()
        {
            return Title;
        }

        public static IEnumerable<SummaryBookListItem> Construct(CorpusItem corpusItem)
        {
            foreach (var corpusFileItem in corpusItem.Files)
            {
                if (corpusFileItem.Path.EndsWith(".lsi"))
                {
                    yield return new SummaryBookListItem
                    {
                        CorpusId = corpusItem.Id,
                        File = corpusFileItem.Path,
                        Title = corpusFileItem.Name
                    };
                }
            }
        }
    }
}