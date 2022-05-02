using System.IO;

namespace LeninSearch.Script.Scripts
{
    public class Fb2CleanupScript : IScript
    {
        public string Id => "fb2-cleanup";
        public string Arguments => "fb2-file, garbage-start-token, garbage-end-token, replace-token";
        public void Execute(params string[] input)
        {
            var fb2File = input[0];
            var garbageStartToken = input[1];
            var garbageEndToken = input[2];
            var replaceToken = input[3];
            var fb2Xml = File.ReadAllText(fb2File);
            
            while (true)
            {
                var garbageStartIndex = fb2Xml.IndexOf(garbageStartToken);

                if (garbageStartIndex == -1) break;

                var garbageEndIndex = fb2Xml.IndexOf(garbageEndToken, garbageStartIndex) + garbageEndToken.Length;

                var before = fb2Xml.Substring(0, garbageStartIndex);
                var after = fb2Xml.Substring(garbageEndIndex);
                fb2Xml = $"{before}{replaceToken}{after}";
            }

            var fixedFile = fb2File.Replace(".fb2", "-fixed.fb2");
            File.WriteAllText(fixedFile, fb2Xml);
        }
    }
}