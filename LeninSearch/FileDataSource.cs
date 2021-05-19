using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LeninSearch.Standard.Core;
using LeninSearch.Standard.Core.OldShit;
using LeninSearch.Standard.Core.Optimized;
using Newtonsoft.Json;

namespace LeninSearch
{
    public class FileDataSource
    {
        private LeninSearchSettings _settings;

        private Task _fileDataLoadTask;

        private readonly Dictionary<string, OptimizedFileData> _fileDatas = new Dictionary<string, OptimizedFileData>();

        public void Refresh(CorpusItem ci)
        {
            _settings = File.Exists(Constants.SettingsJsonPath)
                ? JsonConvert.DeserializeObject<LeninSearchSettings>(File.ReadAllText(Constants.SettingsJsonPath))
                : new LeninSearchSettings();

            if (_settings.PreloadFiles)
            {
                _fileDataLoadTask = Task.Run(() =>
                {
                    for (var i = 0; i < ci.Files.Count; i++)
                    {
                        var cfi = ci.Files[i];
                        var ofd = LsUtil.LoadOptimized($"corpus\\{cfi.Path}", CancellationToken.None);
                        _fileDatas.Add(cfi.Path, ofd);
                    }
                });
            }
            else
            {
                _fileDatas.Clear();
                _fileDataLoadTask = null;
            }
        }

        public OptimizedFileData Get(CorpusFileItem cfi)
        {
            if (_settings.PreloadFiles)
            {
                _fileDataLoadTask?.Wait();
                return _fileDatas[cfi.Path];
            }

            return LsUtil.LoadOptimized($"corpus\\{cfi.Path}", CancellationToken.None);
        }
    }
}