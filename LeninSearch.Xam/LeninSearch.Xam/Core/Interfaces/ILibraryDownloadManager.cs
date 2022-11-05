using System;
using LeninSearch.Standard.Core.Corpus;

namespace LeninSearch.Xam.Core.Interfaces
{
    public interface ILibraryDownloadManager
    {
        event EventHandler<LibraryDownloadCompleteEventArgs> Completed;
        event EventHandler<LibraryDownloadProgressEventArgs> Progress;
        event EventHandler<LibraryDownloadErrorEventArgs> Error;
        void StartDownload(CorpusItem update);
        void CancelDownload(string corpusId);
    }
}