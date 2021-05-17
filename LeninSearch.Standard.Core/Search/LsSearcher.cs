using System.Collections.Generic;
using System.Linq;
using LeninSearch.Standard.Core.Oprimized;

namespace LeninSearch.Standard.Core.Search
{
    public class LsSearcher
    {
        public List<ParagraphSearchResult> SearchParagraphs(LsIndexData lsIndexData, SearchRequest request)
        {
            var allTokens = request.Ordered.Concat(request.NonOrdered).ToList();

            if (allTokens.Any(t => t.WordIndexes.Count == 0)) return new List<ParagraphSearchResult>();

            var chains = GetWordIndexChains(allTokens);

            var candidateParagraphIndexes = new Dictionary<WordIndexChain, List<ushort>>();

            foreach (var chain in chains)
            {
                var currentParagraphIndexList = lsIndexData.WordParagraphData[chain.WordIndexes[0]].Select(wpd => wpd.ParagraphIndex).ToList();
                for (var i = 1; i < chain.WordIndexes.Count; i++)
                {
                    var paragraphIndexList = lsIndexData.WordParagraphData[chain.WordIndexes[i]].Select(wpd => wpd.ParagraphIndex).ToList();
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
                        var currentParagraphDatas = lsIndexData.WordParagraphData[orderedWords[0]].Where(wpd => wpd.ParagraphIndex == paragraphIndex).ToList();
                        for (var i = 1; i < orderedWords.Count; i++)
                        {
                            var nextParagraphDatas = lsIndexData.WordParagraphData[orderedWords[i]].Where(wpd => wpd.ParagraphIndex == paragraphIndex).ToList();
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

                        searchResults.Add(new ParagraphSearchResult(paragraphIndex, chain));

                        ParagraphMismatch: ;
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
                        searchResults.Add(new ParagraphSearchResult(paragraphIndex, chain));
                    }
                }
            }

            return searchResults;
        }

        private List<WordIndexChain> GetWordIndexChains(List<SearchToken> tokens)
        {
            var chains = new List<WordIndexChain>();

            foreach (var token in tokens)
            {
                if (chains.Count == 0)
                {
                    foreach (var wordIndex in token.WordIndexes)
                    {
                        var chain = new WordIndexChain(wordIndex);
                        chains.Add(chain);
                    }
                }
                else
                {
                    var chainsTemplate = new List<WordIndexChain>(chains);
                    foreach (var chain in chains)
                    {
                        chain.WordIndexes.Add(token.WordIndexes[0]);    
                    }
                    for (var i = 1; i < token.WordIndexes.Count; i++)
                    {
                        var addChains = new List<WordIndexChain>(chainsTemplate);
                        foreach (var chain in addChains)
                        {
                            chain.WordIndexes.Add(token.WordIndexes[i]);
                        }
                        chains.AddRange(addChains);
                    }
                }
            }

            return chains;
        }
    }
}