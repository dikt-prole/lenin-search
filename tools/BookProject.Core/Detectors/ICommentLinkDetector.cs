using System.Collections.Generic;
using System.Drawing;
using BookProject.Core.Settings;

namespace BookProject.Core.Detectors
{
    public interface ICommentLinkDetector
    {
        Rectangle[] Detect(Bitmap image, DetectCommentLinkSettings settings, Rectangle[] excludeAreas, Dictionary<string, object> internalValues);
    }
}