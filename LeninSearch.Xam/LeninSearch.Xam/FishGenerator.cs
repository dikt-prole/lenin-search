using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using LeninSearch.Standard.Core.Corpus;
using LeninSearch.Standard.Core.Corpus.Lsi;
using LeninSearch.Standard.Core.Reporting.FishReport;
using LeninSearch.Standard.Core.Search;
using LeninSearch.Xam.Core;

namespace LeninSearch.Xam
{
    public static class FishGenerator
    {
        public static string GenerateSearchReportHtmlFile(PartialParagraphSearchResult ppsr, CorpusItem ci, string query, ILsiProvider lsiProvider)
        {
            try
            {
                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MainPage)).Assembly;
                var stream = assembly.GetManifestResourceStream("LeninSearch.Xam.searchreport.html");
                var fishTemplate = "";
                using (var reader = new StreamReader(stream))
                {
                    fishTemplate = reader.ReadToEnd();
                }

                var fishHtml = $"<h1>Lenin Search Report - {ci.Name} ({query})</h1>";
                var resultIndex = 1;

                foreach (var file in ppsr.Files())
                {
                    var cfi = ci.GetFileByPath(file);
                    var lsiData = lsiProvider.GetLsiData(ci.Id, file);
                    var words = lsiProvider.Words(ci.Id);

                    fishHtml += $"<h2>{cfi.Name}</h2>";
                    var fileResults = ppsr.FileResults(file);
                    foreach (var searchResult in fileResults)
                    {
                        var paragraph = lsiData.Paragraphs[searchResult.ParagraphIndex];
                        var lsiSpans = paragraph.GetSpans(searchResult).Where(s => s.Type != LsiSpanType.Comment);
                        var spans = lsiSpans.Select(lsis => FishReportSpan.From(lsis, words).ToHtmlSpan()).ToList();
                        var paragraphText = string.Join(" ", spans);

                        var headings = lsiData.GetHeadingsDownToZero(searchResult.ParagraphIndex);
                        var linkText = $"{resultIndex} {string.Join(" - ", headings.Select(h => h.GetText(words)))}";
                        fishHtml += $"<h3>{linkText}</h3>";
                        fishHtml += $"<p>{paragraphText}</p>";
                        fishHtml += "<p style='margin-top: 20px;'>(Aut) -</p>";
                        resultIndex++;
                    }
                }

                var fishFile = Path.Combine(Path.GetTempPath(), $"lenin-search-fish-report-{Guid.NewGuid().ToString("N").Substring(0, 8)}.html");

                fishHtml = fishTemplate.Replace("[content]", fishHtml);

                File.WriteAllText(fishFile, fishHtml);

                return fishFile;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }
    }
}