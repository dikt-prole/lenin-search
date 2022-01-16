using System;
using LeninSearch.Standard.Core.Corpus.Lsi;
using LeninSearch.Xam.Controls;
using Xamarin.Forms;

namespace LeninSearch.Xam.ParagraphAdder
{
    public class ParagraphGestureDecorator : IParagraphViewBuilder
    {
        private readonly IParagraphViewBuilder _builder;
        public event EventHandler<LsiParagraph> ParagraphDoubleTapped;
        
        public ParagraphGestureDecorator(IParagraphViewBuilder builder)
        {
            _builder = builder;
        }

        public View Build(LsiParagraph p, State state, string[] dictionaryWords)
        {
            var view = _builder.Build(p, state, dictionaryWords);
            if (!p.IsImage)
            {
                AttachTapGesture(p, view, state);
            }
            return view;
        }

        private void AttachTapGesture(LsiParagraph p, View view, State state)
        {
            var label = FindLabel(view);

            if (label == null) return;

            var doubleTapRecognizer = new TapGestureRecognizer {NumberOfTapsRequired = 2};
            doubleTapRecognizer.Tapped += (sender, args) => { ParagraphDoubleTapped?.Invoke(this, p); };
            label.GestureRecognizers.Add(doubleTapRecognizer);
        }

        private ExtendedLabel FindLabel(View view)
        {
            if (view is StackLayout stack) return stack.Children[0] as ExtendedLabel;

            return null;
        }
    }
}