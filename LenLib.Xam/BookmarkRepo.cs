using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LenLib.Xam.Core;
using Newtonsoft.Json;

namespace LenLib.Xam
{
    public static class BookmarkRepo
    {
        private static List<Bookmark> _bookmarks;

        private static string FilePath(Guid id) => $"{Options.BookmarkFolder}/{id}.json";

        private static void LoadBookmarks()
        {
            if (_bookmarks == null)
            {
                if (Directory.Exists(Options.BookmarkFolder))
                {
                    var jsonFiles = Directory.GetFiles(Options.BookmarkFolder, "*.json");
                    _bookmarks = jsonFiles.Select(jf => JsonConvert.DeserializeObject<Bookmark>(File.ReadAllText(jf)))
                        .Where(b => Options.CorpusExists(b.CorpusItemId))
                        .ToList();
                }
                else
                {
                    _bookmarks = new List<Bookmark>();
                }
            }
        }

        private static void WriteBookmark(Bookmark bm)
        {
            if (!Directory.Exists(Options.BookmarkFolder))
            {
                Directory.CreateDirectory(Options.BookmarkFolder);
            }

            var filePath = $"{Options.BookmarkFolder}/{bm.Id}.json";
            var json = JsonConvert.SerializeObject(bm);
            File.WriteAllText(filePath, json);
        }

        public static IEnumerable<Bookmark> GetAll()
        {
            LoadBookmarks();

            foreach (var bm in _bookmarks)
            {
                yield return bm;
            }
        }

        public static void Add(Bookmark bm)
        {
            LoadBookmarks();

            _bookmarks.Add(bm);

            WriteBookmark(bm);
        }

        public static void Delete(Guid id)
        {
            LoadBookmarks();

            _bookmarks = _bookmarks.Where(bm => bm.Id != id).ToList();

            var filePath = FilePath(id);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}