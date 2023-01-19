using System.Drawing;
using System.IO;

namespace LeninSearch.Studio.WinForms.ImageRendering
{
    public interface IImageRenderer
    {
        void RenderJpeg(string imageFile, Stream outStream);
    }
}