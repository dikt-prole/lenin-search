using System.Collections.Generic;
using System.Drawing;
using BookProject.Core.Settings;

namespace BookProject.Core.Detectors
{
    public interface IImageDetector
    {
        Rectangle[] Detect(Bitmap image, DetectImageSettings settings, Rectangle[] excludeAreas, Dictionary<string, object> internalValues);
    }
}