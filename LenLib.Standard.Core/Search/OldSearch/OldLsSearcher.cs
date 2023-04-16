using System.Collections.Generic;
using System.Linq;
using LenLib.Standard.Core.Corpus.Lsi;

namespace LenLib.Standard.Core.Search.OldSearch
{
    public class OldLsSearcher
    {
        public List<SearchUnit> SearchParagraphs(LsiData lsiData, SearchQuery query)
        {
            return SearchParagraphData(lsiData.WordParagraphData, query);
        }

        public List<SearchUnit> SearchHeadings(LsiData lsiData, SearchQuery query)
        {
            var headingIndexes = lsiData.Headings.Select(hd => hd.Index).ToHashSet();

            var searchResult = SearchParagraphData(lsiData.WordParagraphData, query);

            searchResult = searchResult.Where(sr => headingIndexes.Contains(sr.ParagraphIndex)).ToList();

            return searchResult;
        }

        public List<SearchUnit> SearchParagraphData(Dictionary<uint, List<LsWordParagraphData>> wordParagraphData, SearchQuery query)
        {
            query = query.Copy();

            var allTokens = query.Ordered.Concat(query.NonOrdered).ToList();
            foreach (var token in allTokens)
            {
                token.WordIndexes = token.WordIndexes.Where(wordParagraphData.ContainsKey).ToList();
                if (token.WordIndexes.Count == 0)
                {
                    return new List<SearchUnit>();
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

            var searchResults = new List<SearchUnit>();
            var orderedCount = query.Ordered.Count;
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
                            searchResult = new SearchUnit(paragraphIndex);                            
                            searchResults.Add(searchResult);
                        }

                        searchResult.AddChain(chain);

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
                            searchResult = new SearchUnit(paragraphIndex);
                            searchResults.Add(searchResult);
                        }

                        searchResult.AddChain(chain);
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