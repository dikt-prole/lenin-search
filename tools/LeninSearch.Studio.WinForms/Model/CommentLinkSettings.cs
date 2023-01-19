using System.Drawing;

namespace LeninSearch.Studio.WinForms.Model
{
    public class CommentLinkSettings
    {
        public int MinWidth { get; set; }
        public int MaxWidth { get; set; }
        public int MinHeight { get; set; }
        public int MaxHeight { get; set; }
        public int MinLineBottomDistance { get; set; }
        public int MaxLineBottomDistance { get; set; }
        public int MinLineTopDistance { get; set; }
        public int MaxLineTopDistance { get; set; }

        public bool SizeMatch(Rectangle rect)
        {
            return (MinWidth <= rect.Width && rect.Width <= MaxWidth) &&
                   (MinHeight <= rect.Height && rect.Height <= MaxHeight);
        }

        public bool LineDistanceMatch(int bottomDistance, int topDistance)
        {
            if (bottomDistance > MaxLineBottomDistance) return false;
            if (bottomDistance < MinLineBottomDistance) return false;
            if (topDistance > MaxLineTopDistance) return false;
            if (topDistance < MinLineTopDistance) return false;
            return true;
        }
    }
}