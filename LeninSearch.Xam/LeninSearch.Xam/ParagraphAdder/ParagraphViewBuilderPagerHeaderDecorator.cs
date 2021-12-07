using LeninSearch.Standard.Core.Optimized;
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

        public View Build(LsParagraph p, State state, string[] dictionaryWords)
        {
            if (p.IsPageNumber)
            {
                var pLabel = new Label { Text = $"-------- {p.PageNumber} --------", TextColor = Color.Black, HorizontalOptions = LayoutOptions.Center };
                pLabel.TabIndex = p.Index;
                return pLabel;
            }

            if (p.IsHeading)
            {
                var headingText = p.GetText(dictionaryWords);
                var pLabel = new Label { Text = headingText, TextColor = Color.Black, HorizontalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
                pLabel.TabIndex = p.Index;
                return pLabel;
            }

            return _builder.Build(p, state, dictionaryWords);
        }
    }
}