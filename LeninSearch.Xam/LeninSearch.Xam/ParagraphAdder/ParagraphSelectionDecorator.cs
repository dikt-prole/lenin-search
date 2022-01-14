using System;
using System.Collections.Generic;
using System.Linq;
using LeninSearch.Standard.Core.Corpus.Lsi;
using LeninSearch.Standard.Core.Search;
using LeninSearch.Xam.Controls;
using LeninSearch.Xam.Effects;
using Xamarin.Forms;

namespace LeninSearch.Xam.ParagraphAdder
{
    public class ParagraphSelectionDecorator : IParagraphViewBuilder
    {
        private readonly IParagraphViewBuilder _builder;
        private readonly ILsiProvider _lsiProvider;

        private readonly Dictionary<ushort, View> _selectedParagraphs = new Dictionary<ushort, View>();

        public event EventHandler<List<ushort>> ParagraphSelectionChanged;

        public List<ushort> SelectedIndexes => _selectedParagraphs.Keys.OrderBy(i => i).ToList();

        public ParagraphSelectionDecorator(IParagraphViewBuilder builder, ILsiProvider lsiProvider)
        {
            _builder = builder;
            _lsiProvider = lsiProvider;
        }

        public View Build(LsiParagraph p, State state, string[] dictionaryWords)
        {
            var view = _builder.Build(p, state, dictionaryWords);
            if (!p.IsImage)
            {
                AttachTapGesture(view, state);
            }
            return view;
        }

        public void ClearSelection()
        {
            foreach (var index in _selectedParagraphs.Keys)
            {
                _selectedParagraphs[index].BackgroundColor = Color.White;
            }

            _selectedParagraphs.Clear();
            var handler = ParagraphSelectionChanged;
            handler?.Invoke(this, SelectedIndexes);
        }

        private void AttachTapGesture(View view, State state)
        {
            var label = FindLabel(view);

            if (label == null) return;

            var tapRecognizer = new TapGestureRecognizer {NumberOfTapsRequired = 1};
            tapRecognizer.Tapped += (s, a) =>
            {
                var pIndex = (ushort)view.TabIndex;
                if (_selectedParagraphs.ContainsKey(pIndex))
                {
                    _selectedParagraphs.Remove(pIndex);
                    var deselectAnimation = new Animation(f => label.BackgroundColor = new Color(1.0, 0, 0, f), 0.15, 0, Easing.SinInOut);
                    deselectAnimation.Commit(label, "deselect", 100);
                }
                else
                {
                    var lsiData = _lsiProvider.GetLsiData(state.CorpusId, state.ReadingFile);
                    var videoId = lsiData.GetVideoId(pIndex);
                    if (videoId != null)
                    {
                        foreach (var index in _selectedParagraphs.Keys)
                        {
                            _selectedParagraphs[index].BackgroundColor = Color.White;
                        }

                        _selectedParagraphs.Clear();
                    }

                    _selectedParagraphs.Add(pIndex, view);
                    var selectAnimation = new Animation(f => label.BackgroundColor = new Color(1.0, 0, 0, f), 0, 0.15, Easing.SinInOut);
                    selectAnimation.Commit(label, "select", 100);
                }

                var handler = ParagraphSelectionChanged;
                handler?.Invoke(this, SelectedIndexes);
            };

            label.GestureRecognizers.Add(tapRecognizer);
        }

        private ExtendedLabel FindLabel(View view)
        {
            if (view is ExtendedLabel label) return label;

            if (view is StackLayout stack) return stack.Children[0] as ExtendedLabel;

            return null;
        }
    }
}