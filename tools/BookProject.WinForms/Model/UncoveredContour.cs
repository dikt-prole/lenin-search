using System.Drawing;
using System.IO;
using BookProject.Core.Models.Book;
using BookProject.Core.Models.Book.Old;

namespace BookProject.WinForms.Model
{
    public class UncoveredContour
    {
        public string ImageFile { get; set; }
        public OldBookProjectWord Word { get; set; }
        public OldBookProjectPage Page { get; set; }
        public Rectangle Rectangle { get; set; }
        public string Filename => Path.GetFileNameWithoutExtension(ImageFile);
    }
}