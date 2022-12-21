using System.IO;

namespace LeninSearch.Script.Scripts
{
    public class AutoRotateScript : IScript
    {
        public string Id => "auto-rotate-canvas";
        public string Arguments => "images-folder";
        public void Execute(params string[] input)
        {
            var imageFolder = input[0];

            var imageFiles = Directory.GetFiles(imageFolder, "*.jpg");
            foreach (var imageFile in imageFiles)
            {
                
            }


        }
    }
}