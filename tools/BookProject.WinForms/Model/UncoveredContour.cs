using System.Drawing;
using System.IO;
using BookProject.Core.Models.Book;

namespace BookProject.WinForms.Model
{
    public class UncoveredContour
    {
        public string ImageFile { get; set; }
        public BookProjectWord Word { get; set; }
        public BookProjectPage Page { get; set; }
        public Rectangle Rectangle { get; set; }
        public string Filename => Path.GetFileNameWithoutExtension(ImageFile);
    }
}