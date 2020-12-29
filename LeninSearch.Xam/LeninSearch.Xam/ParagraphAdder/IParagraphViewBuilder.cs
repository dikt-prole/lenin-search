using LeninSearch.Xam.Core;
using LeninSearch.Xam.Core.Oprimized;
using Xamarin.Forms;

namespace LeninSearch.Xam.ParagraphAdder
{
    public interface IParagraphViewBuilder
    {
        View Build(OptimizedParagraph p, State state);
    }
}