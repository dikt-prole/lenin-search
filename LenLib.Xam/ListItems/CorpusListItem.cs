using System.Linq;
using LenLib.Standard.Core.Corpus;

namespace LenLib.Xam.ListItems
{
    public class CorpusListItem
    {
        public string CorpusIconSource { get; set; }
        public string CorpusTitle { get; set; }
        public string CorpusId { get; set; }
        public bool IsDeleteAllowed { get; set; }
        public CorpusListItem Self => this;

        public static CorpusListItem FromCorpusItem(CorpusItem corpusItem)
        {
            return new CorpusListItem
            {
                CorpusId = corpusItem.Id,
                CorpusTitle = $"{corpusItem.Name} ({corpusItem.Files.Count(cfi => cfi.Path.EndsWith(".lsi"))})",
                CorpusIconSource = Options.IconFile(corpusItem.Id),
                IsDeleteAllowed = !Options.InitialSeries.Contains(corpusItem.Series)
            };
        }
    }
}