using System.Collections.Generic;
using System.Linq;

namespace LeninSearch.Standard.Core.Search
{
    public class SearchResult
    {
        public bool Success => Error == null;
        public string Error { get; set; }

        // var units = Units["file"]["query"]
        public Dictionary<string, Dictionary<string, List<SearchUnit>>> Units { get; set; }

        public List<SearchUnit> GetUnitsByFile(string file)
        {
            return Units[file].SelectMany(kvp => kvp.Value).ToList();
        }
        public List<SearchUnit> GetUnitsByQuery(string query)
        {
            var units = new List<SearchUnit>();
            foreach (var file in Units.Keys)
            {
                units.AddRange(Units[file][query]);
            }

            return units;
        }

        public List<SearchUnit> GetUnitsByFileAndQuery(string file, string query)
        {
            return Units[file][query];
        }

        public List<string> GetAllFiles()
        {
            return Units.Keys.ToList();
        }

        public List<string> GetAllQueries()
        {
            var queries = new List<string>();
            foreach (var file in Units.Keys)
            {
                queries.AddRange(Units[file].Keys);
            }

            return queries;
        }
    }
}