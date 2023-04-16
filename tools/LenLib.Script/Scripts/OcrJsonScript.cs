using System;

namespace LenLib.Script.Scripts
{
    public class OcrJsonScript : IScript
    {
        public string Id => "ocr-json";
        public string Arguments => "image";
        public void Execute(params string[] input)
        {
            throw new NotImplementedException();

            //var bookFolders = input;
            //var blockService = new YandexVisionOcrLineService();
            //foreach (var bookFolder in bookFolders)
            //{
            //    Console.WriteLine($"Processing book folder: {bookFolder}");
            //    var sw = new Stopwatch(); sw.Start();
            //    var ocrData = OcrData.Empty();
            //    var imageFiles = Directory.GetFiles(bookFolder)
            //        .Where(f => f.EndsWith(".png") || f.EndsWith(".jpg") || f.EndsWith(".jpeg"));
            //    foreach (var imageFile in imageFiles)
            //    {
            //        var linesResult = blockService.GetOcrPageAsync(imageFile).Result;
            //        if (!linesResult.Success)
            //        {
            //            Console.WriteLine(linesResult.Error);
            //            return;
            //        }

            //        ocrData.Lines.AddRange(linesResult.Lines);
            //    }

            //    ocrData.Save(bookFolder);
            //    sw.Stop();
            //    Console.WriteLine($"Ready in {sw.Elapsed}");
            //}
        }
    }
}