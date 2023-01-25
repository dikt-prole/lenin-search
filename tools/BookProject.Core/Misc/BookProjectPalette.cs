using System.Drawing;
using BookProject.Core.Models.Book;

namespace BookProject.Core.Misc
{
    public static class BookProjectPalette
    {
        public static Color GetColor(BookProjectLabel? label)
        {
            if (label == null) return Color.Gray;

            switch (label.Value)
            {
                case BookProjectLabel.PStart:
                    return Color.Green;

                case BookProjectLabel.PMiddle:
                    return Color.MediumAquamarine;

                case BookProjectLabel.Garbage:
                    return Color.Brown;

                case BookProjectLabel.Image:
                    return Color.Orange;

                case BookProjectLabel.Comment:
                    return Color.DodgerBlue;
                
                case BookProjectLabel.Title:
                    return Color.Red;
            }

            return Color.Black; 
        }

        public static Color ImageBlockColor => Color.Coral;
        public static Color TitleBlockColor => Color.HotPink;
        public static Color GarbageBlockColor => Color.Brown;
    }
}