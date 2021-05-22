using System.IO;
using System.Threading;
using LeninSearch.Standard.Core;
using LeninSearch.Standard.Core.Optimized;

namespace LeninSearch.Xam.Core
{
    public static class LsIndexDataSource
    {
        private static LsIndexData _currentLsiData;
        private static string _currentLsiPath;
        public static LsIndexData Get(string lsiPath)
        {
            FileUtil.Indexing().Wait();

            if (_currentLsiPath == lsiPath && _currentLsiData != null)
            {
                return _currentLsiData;
            }

            var lsiBytes = File.ReadAllBytes($"{FileUtil.CorpusFolder}/{lsiPath}");
            _currentLsiData = LsIndexUtil.FromLsIndexBytes(lsiBytes);
            _currentLsiPath = lsiPath;

            return _currentLsiData;
        }

        public static void Clear()
        {
            _currentLsiData = null;
        }
    }
}