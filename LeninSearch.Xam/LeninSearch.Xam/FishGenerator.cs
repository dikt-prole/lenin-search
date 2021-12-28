using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LeninSearch.Standard.Core.Corpus;
using LeninSearch.Standard.Core.Optimized;
using LeninSearch.Standard.Core.Search;
using LeninSearch.Xam.Core;
using Xamarin.Forms;

namespace LeninSearch.Xam
{
    public static class FishGenerator
    {
        public static string GenerateFishHtmlFile(PartialParagraphSearchResult ppsr, CorpusItem ci, string query, ILsiProvider lsiProvider)
        {
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MainPage)).Assembly;
            var stream = assembly.GetManifestResourceStream("LeninSearch.Xam.fish.html");
            var fishTemplate = "";
            using (var reader = new StreamReader(stream))
            {
                fishTemplate = reader.ReadToEnd();
            }

            var fishHtml = $"<h1>Lenin Search Fish Report - {ci.Name} ({query})</h1>";
            var resultIndex = 1;

            foreach (var file in ppsr.Files())
            {
                var cfi = ci.GetFileByPath(file);
                var lsData = lsiProvider.GetLsiData(ci.Id, file).LsData;
                var words = lsiProvider.Words(ci.Id);

                fishHtml += $"<h2>{cfi.Name}</h2>";
                var fileResults = ppsr.FileResults(file);
                foreach (var searchResult in fileResults)
                {
                    var paragraph = lsData.Paragraphs[searchResult.ParagraphIndex];

                    var chain = searchResult.WordIndexChains[0];
                    var selection = chain.WordIndexes.Select(wi => words[wi].ToLower()).ToArray();
                    var textParts = GetTextParts(paragraph.GetText(words), selection).ToList();
                    var paragraphText = string.Join("", textParts);
                    var headings = lsData.GetHeadingsDownToZero(searchResult.ParagraphIndex);
                    var page = lsData.GetClosestPage(searchResult.ParagraphIndex);
                    var linkText = $"";
                    if (page != null || headings.Any())
                    {
                        var headingText = headings.Count > 0
                            ? string.Join(" - ", headings.Select(h => h.GetText(words)))
                            : null;

                        linkText = page == null
                            ? $"{resultIndex}. {headingText}"
                            : string.IsNullOrEmpty(headingText)
                                ? $"{resultIndex}. стр. {page}"
                                : $"{resultIndex}. стр. {page}, {headingText}";
                    }

                    fishHtml += $"<h3>{linkText}</h3>";
                    fishHtml += $"<p>{paragraphText}</p>";
                    resultIndex++;
                }
            }

            var fishFile = Path.Combine(Path.GetTempPath(), $"lenin-search-fish-report-{Guid.NewGuid().ToString("N").Substring(0, 8)}.html");

            fishHtml = fishTemplate.Replace("[content]", fishHtml);

            File.WriteAllText(fishFile, fishHtml);

            return fishFile;
        }

        private static IEnumerable<string> GetTextParts(string text, string[] selection)
        {
            var lowerText = text.ToLower();

            var selectionIndexes = new List<Tuple<int, string>>();
            foreach (var token in selection)
            {
                var selectionIndex = lowerText.IndexOf(token, 0);
                while (selectionIndex >= 0)
                {
                    selectionIndexes.Add(new Tuple<int, string>(selectionIndex, token));
                    selectionIndex = lowerText.IndexOf(token, selectionIndex + token.Length);
                }
            }

            var startIndex = 0;
            foreach (var si in selectionIndexes.OrderBy(si => si.Item1))
            {
                if (si.Item1 < 0) continue;

                if (si.Item1 > startIndex)
                {
                    var fragment = text.Substring(startIndex, si.Item1 - startIndex);
                    yield return fragment;
                    startIndex = si.Item1;
                }

                var sFragment = text.Substring(startIndex, si.Item2.Length);
                yield return $"<b>{sFragment}</b>";
                startIndex = startIndex + sFragment.Length;
            }

            if (startIndex < text.Length - 1)
            {
                var fragment = text.Substring(startIndex);
                yield return fragment;
            }
        }
    }
}