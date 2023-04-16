using LenLib.Standard.Core.Search;

namespace LenLib.Xam.Core
{
    public static class LsiExtensions
    {
        public static string[] Words(this ILsiProvider lsiProvider, string corpusId)
        {
            return lsiProvider.GetDictionary(corpusId).Words;
        }
    }
}