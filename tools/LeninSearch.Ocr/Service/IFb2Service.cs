namespace LeninSearch.Ocr.Service
{
    public interface IFb2Service
    {
        void GenerateFb2File(string ocrFolder, string fb2File, string fb2Template);
    }
}