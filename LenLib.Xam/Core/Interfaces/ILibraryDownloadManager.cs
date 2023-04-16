using System;
using LenLib.Standard.Core.Corpus;

namespace LenLib.Xam.Core.Interfaces
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