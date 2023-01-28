using System.Drawing;

namespace BookProject.Core.ImageRendering
{
    public interface IImageRenderer
    {
        void Render(Bitmap originalBitmap, Graphics g);
    }
}