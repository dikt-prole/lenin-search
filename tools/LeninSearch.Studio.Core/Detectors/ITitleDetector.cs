using System.Collections.Generic;
using System.Drawing;
using LeninSearch.Studio.Core.Settings;

namespace LeninSearch.Studio.Core.Detectors
{
    public interface ITitleDetector
    {
        Rectangle[] Detect(string imageFile, DetectTitleSettings settings, Rectangle[] excludeAreas, Dictionary<string, object> internalValues);
    }
}