using System.Collections.Generic;
using System.Drawing;
using BookProject.Core.Settings;

namespace BookProject.Core.Detectors
{
    public interface ICommentLinkNumberDetector
    {
        Rectangle[] Detect(Bitmap image, DetectCommentLinkNumberSettings settings, Rectangle[] excludeAreas, Dictionary<string, object> internalValues);
    }
}