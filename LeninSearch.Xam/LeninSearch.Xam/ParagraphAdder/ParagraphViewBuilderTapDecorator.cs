using System;
using System.Collections.Generic;
using System.Linq;
using LeninSearch.Standard.Core.Oprimized;
using Xamarin.Forms;

namespace LeninSearch.Xam.ParagraphAdder
{
    public class ParagraphViewBuilderTapDecorator : IParagraphViewBuilder
    {
        private readonly IParagraphViewBuilder _builder;

        private readonly Dictionary<ushort, View> _selectedParagraphs = new Dictionary<ushort, View>();

        public event EventHandler<List<ushort>> ParagraphSelectionChanged;

        public List<ushort> SelectedIndexes => _selectedParagraphs.Keys.OrderBy(i => i).ToList();

        public ParagraphViewBuilderTapDecorator(IParagraphViewBuilder builder)
        {
            _builder = builder;
        }

        public View Build(OptimizedParagraph p, State state)
        {
            var view = _builder.Build(p, state);
            AttachTapGesture(view);
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

        private void AttachTapGesture(View view)
        {
            var tapRecognizer = new TapGestureRecognizer();
            tapRecognizer.NumberOfTapsRequired = 1;
            tapRecognizer.Tapped += (s, e) =>
            {
                var pIndex = (ushort)view.TabIndex;
                if (_selectedParagraphs.ContainsKey(pIndex))
                {
                    _selectedParagraphs.Remove(pIndex);
                    var deselectAnimation = new Animation(f => view.BackgroundColor = new Color(1.0, 0, 0, f), 0.25, 0, Easing.SinInOut);
                    deselectAnimation.Commit(view, "deselect", 200);
                }
                else
                {
                    _selectedParagraphs.Add(pIndex, view);
                    var selectAnimation = new Animation(f => view.BackgroundColor = new Color(1.0, 0, 0, f), 0, 0.25, Easing.SinInOut);
                    selectAnimation.Commit(view, "deselect", 200);
                }

                var handler = ParagraphSelectionChanged;
                handler?.Invoke(this, SelectedIndexes);
            };
            view.GestureRecognizers.Add(tapRecognizer);
        }
    }
}