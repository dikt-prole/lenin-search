using System.Collections.Generic;
using System.Drawing;
using BookProject.Core.Settings;

namespace BookProject.Core.Detectors
{
    public interface ICommentLinkNumberDetector
    {
        Rectangle[] Detect(string imageFile, DetectCommentLinkNumberSettings settings, Rectangle[] excludeAreas, Dictionary<string, object> internalValues);
    }
}