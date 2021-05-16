using LeninSearch.Standard.Core.Oprimized;
using LeninSearch.Xam.Core;
using Xamarin.Forms;

namespace LeninSearch.Xam.ParagraphAdder
{
    public class ParagraphViewBuilderPagerHeaderDecorator : IParagraphViewBuilder
    {
        private readonly IParagraphViewBuilder _builder;

        public ParagraphViewBuilderPagerHeaderDecorator(IParagraphViewBuilder builder)
        {
            _builder = builder;
        }

        public View Build(OptimizedParagraph p, State state)
        {
            if (p.IsPageNumber)
            {
                var pLabel = new Label { Text = $"-------- {p.PageNumber} --------", TextColor = Color.Black, HorizontalOptions = LayoutOptions.Center };
                pLabel.TabIndex = p.Index;
                return pLabel;
            }

            if (p.IsHeader)
            {
                var ofd = OptimizedFileDataSource.Get(state.ReadingFile);
                //var headerText = ofd.GetHeader(p.Index).GetText();
                var headerText = "NOT IMPLEMENTED";
                var pLabel = new Label { Text = headerText, TextColor = Color.Black, HorizontalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
                pLabel.TabIndex = p.Index;
                return pLabel;
            }

            return _builder.Build(p, state);
        }
    }
}