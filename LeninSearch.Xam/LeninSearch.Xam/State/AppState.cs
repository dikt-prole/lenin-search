using System.Collections.Generic;
using System.IO;
using System.Linq;
using LeninSearch.Standard.Core.Corpus;
using LeninSearch.Standard.Core.Search;
using Newtonsoft.Json;

namespace LeninSearch.Xam.State
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
    }
}
