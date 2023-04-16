using System.ComponentModel;
using System.Runtime.CompilerServices;
using LenLib.Standard.Core.Corpus;

namespace LenLib.Xam.ListItems
{
    public class LibraryListItem : INotifyPropertyChanged
    {
        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                if (value == _text) return;
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }


        public LibraryListItem Self => this;
        public CorpusItem Update { get; set; }


        private bool _isDownloading;
        public bool IsDownloading
        {
            get => _isDownloading;
            set
            {
                if (value == _isDownloading) return;
                _isDownloading = value;
                OnPropertyChanged(nameof(IsDownloading));
                OnPropertyChanged(nameof(IsNotDownloading));
            }
        }

        public bool IsNotDownloading => !IsDownloading;

        public static LibraryListItem FromCorpusItem(CorpusItem corpusItem)
        {
            return new LibraryListItem
            {
                Update = corpusItem,
                Text = corpusItem.ToString()
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}