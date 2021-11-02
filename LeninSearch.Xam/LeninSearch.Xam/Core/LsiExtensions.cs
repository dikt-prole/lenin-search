using LeninSearch.Standard.Core.Search;

namespace LeninSearch.Xam.Core
{
    public static class LsiExtensions
    {
        public static string[] Words(this ILsiProvider lsiProvider)
        {
            return lsiProvider.GetDictionary(Settings.CorpusVersion).Words;
        }
    }
}