using System.Collections.Generic;
using System.Linq;
using LeninSearch.Standard.Core.Corpus.Lsi;
using LeninSearch.Standard.Core.Search;
using LeninSearch.Xam.Controls;
using LeninSearch.Xam.Core;
using Xamarin.Forms;

namespace LeninSearch.Xam.ParagraphAdder
{
    public class TextMenuDecorator : IParagraphViewBuilder
    {
        private readonly IParagraphViewBuilder _builder;
        private readonly TextActions _textActions;
        private readonly ILsiProvider _lsiProvider;

        private readonly Dictionary<LsiParagraph, View> _selectedParagraphs = new Dictionary<LsiParagraph, View>();

        public TextMenuDecorator(IParagraphViewBuilder builder, TextActions textActions, ILsiProvider lsiProvider)
        {
            _builder = builder;
            _textActions = textActions;
            _lsiProvider = lsiProvider;
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

        public void ClearSelection()
        {
            foreach (var p in _selectedParagraphs.Keys)
            {
                _selectedParagraphs[p].BackgroundColor = Color.White;
            }
            _selectedParagraphs.Clear();
        }

        private void AttachTapGesture(LsiParagraph p, View view, State state)
        {
            var label = FindLabel(view);

            if (label == null) return;

            var tapRecognizer = new TapGestureRecognizer {NumberOfTapsRequired = 1};
            tapRecognizer.Tapped += (s, a) =>
            {
                if (_selectedParagraphs.ContainsKey(p))
                {
                    _selectedParagraphs.Remove(p);
                    var deselectAnimation = new Animation(f => label.BackgroundColor = new Color(1.0, 0, 0, f), 0.15, 0, Easing.SinInOut);
                    deselectAnimation.Commit(label, "deselect", 100);
                    RemoveTextMenu(view);
                }
                else
                {
                    _selectedParagraphs.Add(p, view);
                    var selectAnimation = new Animation(f => label.BackgroundColor = new Color(1.0, 0, 0, f), 0, 0.15, Easing.SinInOut);
                    selectAnimation.Commit(label, "select", 100);
                    AddTextMenu(view, p, state);
                }
            };
            label.GestureRecognizers.Add(tapRecognizer);
        }

        private ExtendedLabel FindLabel(View view)
        {
            if (view is StackLayout stack) return stack.Children[0] as ExtendedLabel;

            return null;
        }

        private void AddTextMenu(View view, LsiParagraph p, State state)
        {
            var wrapperStack = view as StackLayout;

            if (wrapperStack == null) return;

            var ci = state.GetCurrentCorpusItem();
            var cfi = state.GetReadingCorpusFileItem();

            var menuStack = ViewFactory.ConstructTextMenu(_textActions, p, () => _selectedParagraphs.Keys.ToList(), ci, cfi, _lsiProvider);

            wrapperStack.Children.Insert(0, menuStack);
        }

        private void RemoveTextMenu(View view)
        {
            var wrapperStack = view as StackLayout;

            if (wrapperStack == null) return;

            var menuStack = wrapperStack.Children[0] as StackLayout;

            if (menuStack == null) return;

            wrapperStack.Children.Remove(menuStack);
        }
    }
}