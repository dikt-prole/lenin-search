using System;
using System.IO;
using System.Text;

namespace LeninSearch.Script.Scripts
{
    public class AutoFixNotes : IScript
    {
        public string Id => "auto-fix-notes";
        public string Arguments => "fb2 folder";
        public void Execute(params string[] input)
        {
            var fb2Folder = input[0];
            var fixedFolder = $"{fb2Folder}\\fixed";
            if (!Directory.Exists(fixedFolder))
            {
                Directory.CreateDirectory(fixedFolder);
            }

            var fb2Files = Directory.GetFiles(fb2Folder, "*.fb2");
            var startToken = "<body name=\"notes\">";
            var endToken = "</body>";
            foreach (var fb2File in fb2Files)
            {
                Console.WriteLine($"Processing: {fb2File}");
                var fb2Text = File.ReadAllText(fb2File, Encoding.UTF8);
                var fb2FileNew = $"{fixedFolder}\\{Path.GetFileName(fb2File)}";

                var notesStartIndex = fb2Text.IndexOf(startToken);

                if (notesStartIndex == -1)
                {
                    File.WriteAllText(fb2FileNew, fb2Text);
                    continue;
                }

                notesStartIndex += startToken.Length;

                var notesEndIndex = fb2Text.IndexOf(endToken, notesStartIndex);
                var notesText = fb2Text.Substring(notesStartIndex, notesEndIndex - notesStartIndex);
                notesText = notesText
                    .Replace("<title>", "")
                    .Replace("</title>", "")
                    .Replace("<p>", "")
                    .Replace("</p>", "")
                    .Replace("<emphasis>", "")
                    .Replace("</emphasis>", "")
                    .Replace("section", "p");
                var fb2TextNew = string.Join("", fb2Text.Substring(0, notesStartIndex), notesText,
                    fb2Text.Substring(notesEndIndex));

                File.WriteAllText(fb2FileNew, fb2TextNew, Encoding.UTF8);
            }
        }
    }
}