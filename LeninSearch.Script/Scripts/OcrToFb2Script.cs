namespace LeninSearch.Script.Scripts
{
    public class OcrToFb2Script : IScript
    {
        public string Id => "ocr-to-fb2";
        public string Arguments => "ocr-json-folder, fb2-path";
        public void Execute(params string[] input)
        {
            var ocrJsonFolder = input[0];
            var fb2Path = input[1];

            // todo: implement ocr => fb2 here
        }
    }
}