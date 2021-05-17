using System.Collections.Generic;
using System.Threading.Tasks;
using LeninSearch.Standard.Core;
using LeninSearch.Standard.Core.Search;

namespace LeninSearch.Xam.Searcher
{
    public interface ISearcher
    {
        Task Search(CorpusItem corpusItem, SearchOptions options, List<SearchParagraphResult> results);
        Task SearchHeaders(CorpusItem corpusItem, SearchOptions options, List<SearchHeaderResult> results);
    }
}