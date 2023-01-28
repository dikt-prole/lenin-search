using System.Drawing;
using System.IO;

namespace BookProject.Core.Utilities
{
    public static class ImageUtility
    {
        public static Bitmap Load(string imagePath)
        {
            return new Bitmap(Image.FromStream(new MemoryStream(File.ReadAllBytes(imagePath))));
        }
    }
}