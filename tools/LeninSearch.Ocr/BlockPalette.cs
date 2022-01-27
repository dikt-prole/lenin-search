using System.Drawing;
using LeninSearch.Ocr.Labeling;

namespace LeninSearch.Ocr
{
    public static class BlockPalette
    {
        public static Color GetColor(OcrBlockLabel? label)
        {
            if (label == null) return Color.Gray;

            switch (label.Value)
            {
                case OcrBlockLabel.Garbage:
                    return Color.Brown;
                case OcrBlockLabel.Image:
                    return Color.Orange;
                case OcrBlockLabel.Comment:
                    return Color.DodgerBlue;
                case OcrBlockLabel.Paragraph:
                    return Color.Green;
                case OcrBlockLabel.Title:
                    return Color.Red;
            }

            return Color.Black; 
        }

        public static Color ImageBlockColor => Color.Coral;
    }
}