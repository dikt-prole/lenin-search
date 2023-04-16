using System.Linq;
using LenLib.Standard.Core.Corpus;

namespace LenLib.Xam.State
{
    public class AppState
    {
        public string ActiveCorpusId { get; set; }
        public int SelectedTabIndex { get; set; }
        public ReadingTabState ReadingTabState { get; set; }
        public SearchTabState SearchTabState { get; set; }
        public SummaryTabState SummaryTabState { get; set; }

        public CorpusItem GetCurrentCorpusItem()
        {
            return Settings.GetCorpusItems().FirstOrDefault(ci => ci.Id == ActiveCorpusId);
        }

        public static AppState Default()
        {
            var corpusItems = Settings.GetCorpusItems().ToList();
            var selectedCorpusItem = corpusItems.First();
            return new AppState
            {
                ActiveCorpusId = selectedCorpusItem.Id,
                SelectedTabIndex = 0,
                ReadingTabState = null,
                SearchTabState = new SearchTabState(),
                SummaryTabState = new SummaryTabState()
            };
        }
    }
}
