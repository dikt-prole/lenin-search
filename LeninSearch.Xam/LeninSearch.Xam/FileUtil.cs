using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace LeninSearch.Xam
{
    public static class FileUtil
    {
        public static string CorpusFolder => Path.Combine(Path.GetTempPath(), "LeninSearch_Corpus");
        public static string LsiFolder => Path.Combine(Path.GetTempPath(), "LeninSearch_Lsi");
        public static string BookmarkFolder => Path.Combine(Path.GetTempPath(), "LeninSearch_Bookmarks");
        public static string StateFolder => Path.Combine(Path.GetTempPath(), $"LeninSearch_State_1");
        public static Task UnzipTask { get; set; }

        public static byte[] ReadCorpusFile(string file)
        {
            var filePath = $"{CorpusFolder}/{file}";
            return File.ReadAllBytes(filePath);
        }

        public static async Task Indexing()
        {
            if (UnzipTask == null) return;

            await UnzipTask;

            UnzipTask = null;
        }
    }
}
