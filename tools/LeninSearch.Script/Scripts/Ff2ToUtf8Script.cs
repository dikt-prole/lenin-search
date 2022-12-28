using System;
using System.IO;
using System.Text;
using Emgu.CV;

namespace LeninSearch.Script.Scripts
{
    public class Ff2ToUtf8Script : IScript
    {
        public string Id => "fb2-to-utf8";
        public string Arguments => "fb2 folder";
        public void Execute(params string[] input)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var fb2Folder = input[0];
            var utf8Folder = $"{fb2Folder}\\utf8";
            if (!Directory.Exists(utf8Folder))
            {
                Directory.CreateDirectory(utf8Folder);
            }

            var fb2Files = Directory.GetFiles(fb2Folder, "*.fb2");
            
            foreach (var fb2File in fb2Files)
            {
                var fb2FileUtf8 = $"{utf8Folder}\\{Path.GetFileName(fb2File)}";
                var firstLine = File.ReadAllLines(fb2File)[0].ToLower();
                if (firstLine.Contains("utf-8"))
                {
                    File.Copy(fb2File, fb2FileUtf8);
                }
                else if (firstLine.Contains("windows-1251"))
                {
                    var windows1251Encoding = Encoding.GetEncoding(1251);
                    File.WriteAllText(fb2FileUtf8, File.ReadAllText(fb2File, windows1251Encoding), Encoding.UTF8);
                }
                else
                {
                    throw new Exception($"Unsupported encoding: {firstLine}");
                }
            }
        }
    }
}