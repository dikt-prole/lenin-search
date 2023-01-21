using System.IO;

namespace LeninSearch.Studio.Core.ImageRendering
{
    public interface IImageRenderer
    {
        void RenderJpeg(string imageFile, Stream outStream, int canvasWidth, int canvasHeight);

        void RenderBmp(string imageFile, Stream outStream, int canvasWidth, int canvasHeight);
    }
}