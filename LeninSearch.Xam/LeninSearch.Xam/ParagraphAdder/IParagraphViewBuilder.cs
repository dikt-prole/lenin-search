using LeninSearch.Standard.Core.Corpus.Lsi;
using Xamarin.Forms;

namespace LeninSearch.Xam.ParagraphAdder
{
    public interface IParagraphViewBuilder
    {
        View Build(LsiParagraph p, State state, string[] dictionaryWords);
    }
}