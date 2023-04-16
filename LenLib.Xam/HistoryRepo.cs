using System.Collections.Generic;
using System.IO;
using System.Linq;
using LenLib.Xam.Core;
using Newtonsoft.Json;

namespace LenLib.Xam
{
    public static class HistoryRepo
    {
        public static List<HistoryItem> GetHistory()
        {
            if (!File.Exists(Settings.HistoryFile)) return new List<HistoryItem>();

            return JsonConvert.DeserializeObject<List<HistoryItem>>(File.ReadAllText(Settings.HistoryFile))
                .Where(hi => Settings.CorpusExists(hi.CorpusId))
                .ToList();
        }

        public static void AddHistory(HistoryItem historyItem)
        {
            var history = GetHistory();

            history.Add(historyItem);

            history = history.OrderByDescending(hi => hi.QueryDateUtc).Take(Settings.UI.MaxHistoryLength).ToList();

            var historyFolder = Path.GetDirectoryName(Settings.HistoryFile);
            if (!Directory.Exists(historyFolder))
            {
                Directory.CreateDirectory(historyFolder);
            }

            File.WriteAllText(Settings.HistoryFile, JsonConvert.SerializeObject(history));
        }
    }
}