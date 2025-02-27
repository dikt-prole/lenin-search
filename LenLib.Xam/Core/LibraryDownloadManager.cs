﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LenLib.Standard.Core.Api;
using LenLib.Standard.Core.Corpus;
using LenLib.Standard.Core.Search;
using LenLib.Xam.Core.Interfaces;

namespace LenLib.Xam.Core
{
    public class LibraryDownloadManager : ILibraryDownloadManager
    {
        private readonly ILsiProvider _lsiProvider;
        private readonly IApiClientV1 _apiClientV1;
        public event EventHandler<LibraryDownloadCompleteEventArgs> Completed;
        public event EventHandler<LibraryDownloadProgressEventArgs> Progress;
        public event EventHandler<LibraryDownloadErrorEventArgs> Error;

        private ConcurrentDictionary<string, CancellationTokenSource> _cancellationTokenSources =
            new ConcurrentDictionary<string, CancellationTokenSource>();

        public LibraryDownloadManager(ILsiProvider lsiProvider, IApiClientV1 apiClientV1)
        {
            _lsiProvider = lsiProvider;
            _apiClientV1 = apiClientV1;
        }

        public void StartDownload(CorpusItem update)
        {
            var cts = new CancellationTokenSource();
            _cancellationTokenSources.TryAdd(update.Id, cts);
            RunDownloadTask(update, cts.Token);
        }

        public void CancelDownload(string corpusId)
        {
            if (!_cancellationTokenSources.ContainsKey(corpusId))
            {
                return;
            }
            _cancellationTokenSources.TryRemove(corpusId, out var cts);
            cts.Cancel();
        }

        private void RunDownloadTask(CorpusItem download, CancellationToken ct)
        {
            Task.Run(() =>
            {
                var sameSeriesCorpusItems = Options.GetFinishedCorpusIds()
                    .Select(cid => _lsiProvider.GetCorpusItem(cid))
                    .Where(ci => ci.Series == download.Series)
                    .ToList();

                // 1. create corpus folder
                var corpusFolder = Path.Combine(Options.CorpusRoot, download.Id);
                if (Directory.Exists(corpusFolder))
                {
                    Directory.Delete(corpusFolder, true);
                }

                Directory.CreateDirectory(corpusFolder);

                // 2. download files
                foreach (var cfi in download.Files)
                {
                    if (ct.IsCancellationRequested)
                    {
                        Directory.Delete(corpusFolder, true);
                        return;
                    }

                    OnProgress(download, cfi.Path);
                    var cfiBytesResult = _apiClientV1.GetFileBytesAsync(download.Id, cfi.Path).Result;
                    if (!cfiBytesResult.Success)
                    {
                        Directory.Delete(corpusFolder, true);
                        _cancellationTokenSources.TryRemove(download.Id, out _);
                        OnError(download, cfiBytesResult.Error);
                        return;
                    }

                    File.WriteAllBytes(Path.Combine(corpusFolder, cfi.Path), cfiBytesResult.Bytes);
                }

                // 3. remove same series corpus
                foreach (var corpusItem in sameSeriesCorpusItems)
                {
                    Directory.Delete(Path.Combine(Options.CorpusRoot, corpusItem.Id), true);
                }

                OnDownloadCompleted(download, sameSeriesCorpusItems);
            });
        }

        protected virtual void OnProgress(CorpusItem download, string file)
        {
            Progress?.Invoke(this, new LibraryDownloadProgressEventArgs
            {
                Download = download,
                File = file
            });
        }

        protected virtual void OnDownloadCompleted(CorpusItem download, List<CorpusItem> removed)
        {
            Completed?.Invoke(this, new LibraryDownloadCompleteEventArgs
            {
                Download = download,
                Removed = removed
            });
        }

        protected virtual void OnError(CorpusItem download, string error)
        {
            Error?.Invoke(this, new LibraryDownloadErrorEventArgs
            {
                Download = download,
                Error = error
            });
        }
    }

    public class LibraryDownloadCompleteEventArgs : EventArgs
    {
        public CorpusItem Download { get; set; }
        public List<CorpusItem> Removed { get; set; }
    }

    public class LibraryDownloadProgressEventArgs : EventArgs
    {
        public CorpusItem Download { get; set; }
        public string File { get; set; }
    }

    public class LibraryDownloadErrorEventArgs : EventArgs
    {
        public CorpusItem Download { get; set; }
        public string Error { get; set; }
    }
}