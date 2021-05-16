using System.IO;
using System.Linq;

namespace LeninSearch.Standard.Core
{
    public static class FileUtil
    {
        public static string GetFileNameWithoutExtension(string file)
        {
            var split = Path.GetFileName(file).Split('.');
            if (split.Length > 1)
            {
                return string.Join('.', split.Take(split.Length - 1));
            }

            return string.Join('.', split);
        }

        public static void UnzipCorpus(string currentFolder, CorpusItem corpusItem)
        {
            //var tempFolder = $"{Path.GetTempPath()}\\{Guid.NewGuid():N}\\{corpusItem.Name}";
            //Directory.CreateDirectory(tempFolder);

            //var zipFile = $"corpus\\{corpusItem.File}";

            //ZipFile.ExtractToDirectory(zipFile, tempFolder);

            //foreach (var file in Directory.GetFiles(tempFolder))
            //{
            //    var extension = file.Split('.').Last();
            //    var newFile = $"{currentFolder}\\{corpusItem.Name} - {FileUtil.GetFileNameWithoutExtension(file)}.{extension}";
            //    File.Move(file, newFile);
            //}

            //Directory.Delete(tempFolder);
        }
    }
}