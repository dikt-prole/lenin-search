using System.Collections.Generic;
using System.Drawing;
using BookProject.Core.Settings;

namespace BookProject.Core.Detectors
{
    public interface IGarbageDetector
    {
        Rectangle[] Detect(Bitmap image, DetectGarbageSettings settings, Rectangle[] excludeAreas, Dictionary<string, object> internalValues);
    }
}