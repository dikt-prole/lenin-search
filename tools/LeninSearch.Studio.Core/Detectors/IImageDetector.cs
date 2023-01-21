using System.Collections.Generic;
using System.Drawing;
using LeninSearch.Studio.Core.Settings;

namespace LeninSearch.Studio.Core.Detectors
{
    public interface IImageDetector
    {
        Rectangle[] Detect(string imageFile, DetectImageSettings settings, Rectangle[] excludeAreas, Dictionary<string, object> internalValues);
    }
}