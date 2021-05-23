using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LeninSearch.Standard.Core.Optimized;

namespace LeninSearch.Standard.Core.Search
{
    public class LsSearcher
    {
        public List<ParagraphSearchResult> SearchParagraphs(LsIndexData lsIndexData, SearchRequest request)
        {
            return SearchParagraphData(lsIndexData.WordParagraphData, request);
        }

        public List<ParagraphSearchResult> SearchHeadings(LsIndexData lsIndexData, SearchRequest request)
        {
            var headingIndexes = lsIndexData.HeadingData.Select(hd => hd.Index).ToHashSet();

            var searchResult = SearchParagraphData(lsIndexData.WordParagraphData, request);

            searchResult = searchResult.Where(sr => headingIndexes.Contains(sr.ParagraphIndex)).ToList();

            return searchResult;
        }

        public List<ParagraphSearchResult> SearchParagraphData(Dictionary<uint, List<LsWordParagraphData>> wordParagraphData, SearchRequest request)
        {
            request = request.Copy();

            var allTokens = request.Ordered.Concat(request.NonOrdered).ToList();
            foreach (var token in allTokens)
            {
                token.WordIndexes = token.WordIndexes.Where(wordParagraphData.ContainsKey).ToList();
                if (token.WordIndexes.Count == 0)
                {
                    return new List<ParagraphSearchResult>();
                }
            }

            var chains = GetWordIndexChains(allTokens);

            var candidateParagraphIndexes = new Dictionary<WordIndexChain, List<ushort>>();

            foreach (var chain in chains)
            {
                var currentParagraphIndexList = wordParagraphData[chain.WordIndexes[0]].Select(wpd => wpd.ParagraphIndex).ToList();
                for (var i = 1; i < chain.WordIndexes.Count; i++)
                {
                    var paragraphIndexList = wordParagraphData[chain.WordIndexes[i]].Select(wpd => wpd.ParagraphIndex).ToList();
                    currentParagraphIndexList = currentParagraphIndexList.Intersect(paragraphIndexList).ToList();
                }

                if (currentParagraphIndexList.Count > 0)
                {
                    candidateParagraphIndexes.Add(chain, currentParagraphIndexList);
                }
            }

            var searchResults = new List<ParagraphSearchResult>();
            var orderedCount = request.Ordered.Count;
            if (orderedCount > 0)
            {
                foreach (var chain in candidateParagraphIndexes.Keys)
                {
                    var orderedWords = chain.WordIndexes.Take(orderedCount).ToList();
                    var paragraphIndexes = candidateParagraphIndexes[chain];
                    foreach (var paragraphIndex in paragraphIndexes)
                    {
                        var currentParagraphDatas = wordParagraphData[orderedWords[0]].Where(wpd => wpd.ParagraphIndex == paragraphIndex).ToList();
                        for (var i = 1; i < orderedWords.Count; i++)
                        {
                            var nextParagraphDatas = wordParagraphData[orderedWords[i]].Where(wpd => wpd.ParagraphIndex == paragraphIndex).ToList();
                            foreach (var cpd in currentParagraphDatas)
                            {
                                foreach (var npd in nextParagraphDatas)
                                {
                                    if (npd.WordPosition - cpd.WordPosition == 1)
                                    {
                                        goto LocalParagraphMatch;
                                    }
                                }
                            }

                            goto ParagraphMismatch;

                            LocalParagraphMatch:
                            currentParagraphDatas = nextParagraphDatas;
                        }

                        var searchResult = searchResults.FirstOrDefault(r => r.ParagraphIndex == paragraphIndex);
                        if (searchResult == null)
                        {
                            searchResult = new ParagraphSearchResult(paragraphIndex);
                            searchResults.Add(searchResult);
                        }
                        else
                        {
                            searchResult.AddChain(chain);
                        }

                        ParagraphMismatch:;
                    }
                }
            }
            else
            {
                foreach (var chain in candidateParagraphIndexes.Keys)
                {
                    var paragraphIndexes = candidateParagraphIndexes[chain];
                    foreach (var paragraphIndex in paragraphIndexes)
                    {
                        var searchResult = searchResults.FirstOrDefault(r => r.ParagraphIndex == paragraphIndex);
                        if (searchResult == null)
                        {
                            searchResult = new ParagraphSearchResult(paragraphIndex);
                            searchResults.Add(searchResult);
                        }
                        else
                        {
                            searchResult.AddChain(chain);
                        }
                    }
                }
            }

            return searchResults;
        }

        private List<WordIndexChain> GetWordIndexChains(List<SearchToken> tokens)
        {
            var chains = tokens[0].WordIndexes.Select(wi => new WordIndexChain(wi)).ToList();

            for (var i = 1; i < tokens.Count; i++)
            {
                var token = tokens[i];
                var chainsTemplate = chains.Select(c => c.Copy()).ToList();
                foreach (var chain in chains)
                {
                    chain.WordIndexes.Add(token.WordIndexes[0]);
                }
                for (var j = 1; j < token.WordIndexes.Count; j++)
                {
                    var addChains = chainsTemplate.Select(c => c.Copy()).ToList();
                    foreach (var chain in addChains)
                    {
                        chain.WordIndexes.Add(token.WordIndexes[j]);
                    }
                    chains.AddRange(addChains);
                }
            }

            return chains;
        }
    }
}