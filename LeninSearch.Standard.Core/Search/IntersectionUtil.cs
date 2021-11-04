using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace LeninSearch.Standard.Core.Search
{
    public static class IntersectionUtil
    {
        public static IEnumerable<ushort> IntersectOrderedLists(List<List<ushort>> orderedLists)
        {
            var indexes = Enumerable.Repeat(0, orderedLists.Count).ToList();

            while (true)
            {
                var max = Enumerable.Range(0, orderedLists.Count).Max(i => orderedLists[i][indexes[i]]);

                if (Enumerable.Range(0, orderedLists.Count).All(i => orderedLists[i][indexes[i]] == max))
                {
                    yield return max;

                    for (var i = 0; i < indexes.Count; i++)
                    {
                        indexes[i] = indexes[i] + 1;

                        if (indexes[i] >= orderedLists[i].Count) goto Exit;
                    }
                }
                else
                {
                    for (var i = 0; i < orderedLists.Count; i++)
                    {
                        while (orderedLists[i][indexes[i]] < max)
                        {
                            indexes[i] = indexes[i] + 1;

                            if (indexes[i] >= orderedLists[i].Count) goto Exit;
                        }
                    }
                }
            }

            Exit: ;
        }

        public static IEnumerable<ushort> IntersectStd(List<List<ushort>> orderedLists)
        {
            if (orderedLists.Count == 0) yield break;

            if (orderedLists.Count == 1)
            {
                foreach (var i in orderedLists[0]) yield return i;
            }

            var result = orderedLists[0];
            for (var i = 1; i < orderedLists.Count; i++)
            {
                result = result.Intersect(orderedLists[i]).ToList();
            }

            foreach (var i in result) yield return i;
        }
    }
}