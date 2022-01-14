using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LeninSearch.Standard.Core.Corpus.Lsi;
using LeninSearch.Standard.Core.Search;
using LeninSearch.Xam.Controls;
using LeninSearch.Xam.Effects;
using Xamarin.Forms;

namespace LeninSearch.Xam.ParagraphAdder
{
    public class ParagraphGestureDecorator : IParagraphViewBuilder
    {
        private readonly IParagraphViewBuilder _builder;
        private readonly ILsiProvider _lsiProvider;

        private readonly Dictionary<ushort, View> _selectedParagraphs = new Dictionary<ushort, View>();

        public event EventHandler<List<ushort>> ParagraphSelectionChanged;

        public event EventHandler<LsiParagraph> ParagraphDoubleTapped;

        public List<ushort> SelectedIndexes => _selectedParagraphs.Keys.OrderBy(i => i).ToList();

        public ParagraphGestureDecorator(IParagraphViewBuilder builder, ILsiProvider lsiProvider)
        {
            _builder = builder;
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
            foreach (var index in _selectedParagraphs.Keys)
            {
                _selectedParagraphs[index].BackgroundColor = Color.White;
            }

            _selectedParagraphs.Clear();
            var handler = ParagraphSelectionChanged;
            handler?.Invoke(this, SelectedIndexes);
        }

        private void AttachTapGesture(LsiParagraph p, View view, State state)
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

            var doubleTapRecognizer = new TapGestureRecognizer {NumberOfTapsRequired = 2};
            doubleTapRecognizer.Tapped += (sender, args) =>
            {
                Debug.WriteLine("Paragraph double tap");
                ParagraphDoubleTapped?.Invoke(this, p);
            };
            label.GestureRecognizers.Add(doubleTapRecognizer);
        }

        private ExtendedLabel FindLabel(View view)
        {
            if (view is ExtendedLabel label) return label;

            if (view is StackLayout stack) return stack.Children[0] as ExtendedLabel;

            return null;
        }
    }
}