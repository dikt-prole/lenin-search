using LeninSearch.Standard.Core.Optimized;
using Xamarin.Forms;

namespace LeninSearch.Xam.ParagraphAdder
{
    public interface IParagraphViewBuilder
    {
        View Build(LsParagraph p, State state, string[] words);
    }
}