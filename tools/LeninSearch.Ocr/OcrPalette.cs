using System.Drawing;
using LeninSearch.Ocr.Model;

namespace LeninSearch.Ocr
{
    public static class OcrPalette
    {
        public static Color GetColor(OcrLabel? label)
        {
            if (label == null) return Color.Gray;

            switch (label.Value)
            {
                case OcrLabel.PStart:
                    return Color.Green;

                case OcrLabel.PMiddle:
                    return Color.MediumAquamarine;

                case OcrLabel.Garbage:
                    return Color.Brown;

                case OcrLabel.Image:
                    return Color.Orange;

                case OcrLabel.Comment:
                    return Color.DodgerBlue;
                
                case OcrLabel.Title:
                    return Color.Red;
            }

            return Color.Black; 
        }

        public static Color ImageBlockColor => Color.Coral;
        public static Color TitleBlockColor => Color.HotPink;
    }
}