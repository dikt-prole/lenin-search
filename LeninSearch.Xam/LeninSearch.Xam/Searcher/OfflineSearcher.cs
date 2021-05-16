using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LeninSearch.Standard.Core;
using LeninSearch.Xam.Core;

namespace LeninSearch.Xam.Searcher
{
    public class OfflineSearcher : ISearcher
    {
        public static bool OneByOne { get; set; }
        public async Task Search(CorpusItem corpusItem, SearchOptions options, List<SearchParagraphResult> results)
        {
            await FileUtil.WaitUnzip();

            if (OneByOne)
            {
                foreach (var cfi in corpusItem.Files)
                {
                    var cfiResults = await RunSearchParagraphTask(cfi, options);
                    results.AddRange(cfiResults);
                }
            }
            else
            {
                var tasks = corpusItem.Files.Select(cfi => RunSearchParagraphTask(cfi, options)).ToList();
                var taskResults = await Task.WhenAll(tasks);
                foreach (var tr in taskResults)
                {
                    results.AddRange(tr);
                }
            }
        }

        public async Task SearchHeaders(CorpusItem corpusItem, SearchOptions options, List<SearchHeaderResult> results)
        {
            await FileUtil.WaitUnzip();

            if (OneByOne)
            {
                foreach (var cfi in corpusItem.Files)
                {
                    var cfiResults = await RunSearchHeaderTask(cfi, options);
                    results.AddRange(cfiResults);
                }
            }
            else
            {
                var tasks = corpusItem.Files.Select(cfi => RunSearchHeaderTask(cfi, options)).ToList();
                var taskResults = await Task.WhenAll(tasks);
                foreach (var tr in taskResults)
                {
                    results.AddRange(tr);
                }
            }
        }

        private Task<List<SearchParagraphResult>> RunSearchParagraphTask(CorpusFileItem cfi, SearchOptions options)
        {
            return Task.Run(() =>
            {
                var fileBytes = FileUtil.ReadCorpusFile(cfi.Path);
                var ofd = LsUtil.LoadOptimized(fileBytes, CancellationToken.None);
                var paragraphs = ofd.FindParagraphs(options, OptimizedDictionary.Instance.Words).ToList();

                return paragraphs.Select(p =>
                    new SearchParagraphResult
                    {
                        Index = p.Index,
                        File = cfi.Path
                    }).ToList();
            });
        }

        private Task<List<SearchHeaderResult>> RunSearchHeaderTask(CorpusFileItem cfi, SearchOptions options)
        {
            return Task.Run(() =>
            {
                var fileBytes = FileUtil.ReadCorpusFile(cfi.Path);
                var ofd = LsUtil.LoadOptimized(fileBytes, CancellationToken.None);
                var headings = ofd.FindHeaders(options).ToList();

                return headings.Select(h =>
                    new SearchHeaderResult
                    {
                        Index = h.Index,
                        File = cfi.Path,
                        Text = h.Text
                    }).ToList();
            });
        }
    }
}