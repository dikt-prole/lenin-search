using System.Threading;
using LeninSearch.Standard.Core;
using LeninSearch.Standard.Core.Oprimized;

namespace LeninSearch.Xam.Core
{
    public static class OptimizedFileDataSource
    {
        private static OptimizedFileData _currentOfd;
        private static string _currentFile;
        public static OptimizedFileData Get(string filePath)
        {
            FileUtil.WaitUnzip().Wait();

            if (_currentFile == filePath && _currentOfd != null)
            {
                return _currentOfd;
            }

            _currentOfd = LsUtil.LoadOptimized($"{FileUtil.CorpusFolder}/{filePath}", CancellationToken.None);
            _currentFile = filePath;

            return _currentOfd;
        }

        public static void Clear()
        {
            _currentOfd = null;
        }
    }
}