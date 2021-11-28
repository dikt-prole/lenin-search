using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using LeninSearch.Standard.Core.Optimized;

namespace LeninSearch.Standard.Core.Search
{
    public class LsSearcher
    {
        private readonly int _tokenIndexCountCutoff;
        private readonly int _resultCountCutoff;

        public LsSearcher(int tokenIndexCountCutoff = int.MaxValue, int resultCountCutoff = int.MaxValue)
        {
            _tokenIndexCountCutoff = tokenIndexCountCutoff;
            _resultCountCutoff = resultCountCutoff;
        }

        public List<ParagraphSearchResult> SearchParagraphs(LsIndexData lsIndexData, SearchQuery query)
        {
            return SearchParagraphData(lsIndexData.WordParagraphData, query);
        }

        public List<ParagraphSearchResult> SearchHeadings(LsIndexData lsIndexData, SearchQuery query)
        {
            var headingIndexes = lsIndexData.HeadingData.Where(h => h.Level == 0).Select(hd => hd.Index).ToHashSet();

            var searchResult = SearchParagraphData(lsIndexData.WordParagraphData, query);

            searchResult = searchResult.Where(sr => headingIndexes.Contains(sr.ParagraphIndex)).ToList();

            return searchResult;
        }

        public List<ParagraphSearchResult> SearchParagraphData(Dictionary<uint, List<LsWordParagraphData>> wordParagraphData, SearchQuery query)
        {
            var sw = new FancyStopwatch();
            var result = new List<ParagraphSearchResult>();

            query = query.Copy();

            sw.Start("token part");

            var allTokens = query.Ordered.Concat(query.NonOrdered).ToList();
            foreach (var token in allTokens)
            {
                token.WordIndexes = token.WordIndexes.Where(wordParagraphData.ContainsKey)
                    .Take(_tokenIndexCountCutoff)
                    .ToList();
                if (token.WordIndexes.Count == 0)
                {
                    return result;
                }
            }

            sw.StopAndReport();

            sw.Start("token word paragraphs part");
            
            var tokeWordParagraphPartSw = new Stopwatch(); tokeWordParagraphPartSw.Start();

            var tokenParagraphWordDatas = new Dictionary<SearchToken, Dictionary<ushort, List<WordData>>>();
            foreach (var searchToken in allTokens)
            {
                var wordDatas = searchToken.WordIndexes.SelectMany(
                    wordIndex => wordParagraphData[wordIndex].Select(wpd =>
                        new WordData(wordIndex, wpd.ParagraphIndex, wpd.WordPosition)));

                var paragraphWordGroups = wordDatas.GroupBy(wd => wd.ParagraphIndex);

                var paragraphWordDatas = new Dictionary<ushort, List<WordData>>();
                foreach (var pwd in paragraphWordGroups)
                {
                    paragraphWordDatas.Add(pwd.Key, pwd.ToList());
                }

                tokenParagraphWordDatas.Add(searchToken, paragraphWordDatas);
            }

            sw.StopAndReport();

            sw.Start("paragraph reorder part");

            var orderedParagraphLists = new List<List<ushort>>();
            foreach (var searchToken in tokenParagraphWordDatas.Keys)
            {
                orderedParagraphLists.Add(tokenParagraphWordDatas[searchToken].Keys.OrderBy(i => i).ToList());
            }

            sw.StopAndReport();

            sw.Start("paragraph intersection part");

            var intersectingParagraphs = IntersectionUtil.IntersectStd(orderedParagraphLists).Distinct().ToList();

            sw.StopAndReport();

            if (intersectingParagraphs.Count == 0) return result;

            sw.Start("word chain part");

            var wordChains = new Dictionary<ushort, List<WordData>>();

            if (query.Ordered.Count > 0)
            {
                foreach (var paragraphIndex in intersectingParagraphs)
                {
                    var wordCombos = GetWordCombos(tokenParagraphWordDatas, query.Ordered, paragraphIndex);

                    var wordCombo = wordCombos.FirstOrDefault(IsOrderedCombo);

                    if (wordCombo == null) continue;

                    wordChains.Add(paragraphIndex, wordCombo);
                }
            }
            else
            {
                foreach (var paragraphIndex in intersectingParagraphs)
                {
                    wordChains.Add(paragraphIndex, new List<WordData>());
                }
            }

            foreach (var token in query.NonOrdered)
            {
                foreach (var paragraphIndex in wordChains.Keys)
                {
                    wordChains[paragraphIndex].Add(tokenParagraphWordDatas[token][paragraphIndex][0]);
                }
            }

            sw.StopAndReport();

            foreach (var paragraphIndex in wordChains.Keys)
            {
                var searchResult = new ParagraphSearchResult(paragraphIndex, wordChains[paragraphIndex]);
                result.Add(searchResult);
            }

            result = result.Take(_resultCountCutoff).ToList();

            return result;
        }

        private bool IsOrderedCombo(List<WordData> wordDatas)
        {
            if (wordDatas.Count == 1) return true;

            for (var i = 1; i < wordDatas.Count; i++)
            {
                if (wordDatas[i].WordPosition <= wordDatas[i - 1].WordPosition) return false;
            }

            return true;
        }

        private IEnumerable<List<WordData>> GetWordCombos(Dictionary<SearchToken, Dictionary<ushort, List<WordData>>> tokenParagraphWordDatas,
            List<SearchToken> orderedTokens, ushort paragraphIndex)
        {
            var wordDataLists = orderedTokens.Select(t => tokenParagraphWordDatas[t][paragraphIndex]).ToList();

            var wordCombos = GetWordCombos(wordDataLists);

            return wordCombos;
        }

        private IEnumerable<List<WordData>> GetWordCombos(List<List<WordData>> wordDataLists)
        {
            if (wordDataLists.Count == 0) yield break;

            var firstWordDataList = wordDataLists[0];

            var reducedWordDataLists = wordDataLists.Skip(1).ToList();

            var reducedCombos = GetWordCombos(reducedWordDataLists).ToList();

            foreach (var wordData in firstWordDataList)
            {
                var firstWordCombo = new List<WordData> {wordData};

                if (reducedCombos.Any())
                {
                    foreach (var reducedWordCombo in reducedCombos)
                    {
                        yield return firstWordCombo.Concat(reducedWordCombo).ToList();
                    }
                }
                else
                {
                    yield return firstWordCombo;
                }
            }
        }
    }
}