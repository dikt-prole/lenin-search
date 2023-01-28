using System.Collections.Generic;
using System.Drawing;
using System.IO;
using BookProject.Core.Settings;

namespace BookProject.Core.Detectors
{
    public interface ITitleDetector
    {
        Rectangle[] Detect(string imageFile, DetectTitleSettings settings, Rectangle[] excludeAreas, Dictionary<string, object> internalValues);
    }
}