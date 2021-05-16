//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection.Emit;
//using LeninSearch.Standard.Core;
//using LeninSearch.Standard.Core.Oprimized;
//using LeninSearch.Xam.Core;
//using Xamarin.Forms;

//namespace LeninSearch.Xam.ParagraphAdder
//{
//    public class OldAndroidParagraphViewBuilder : IParagraphViewBuilder
//    {
//        public View Build(OptimizedParagraph p)
//        {
//            var flexLayout = new FlexLayout
//            {
//                Direction = FlexDirection.Row,
//                AlignItems = FlexAlignItems.End,
//                JustifyContent = FlexJustify.SpaceBetween,
//                Wrap = FlexWrap.Wrap,
//                Margin = new Thickness(0, 5, 0, 0)
//            };

//            var words = TextUtil.GetSpaceSplit(p.GetWords().ToList());
//            foreach (var word in words)
//            {
//                var wLabel = new Label {Text = word, TextColor = Color.Black};
//                flexLayout.Children.Add(wLabel);
//            }

//            // you need to get your hands dirty when no option to justify
//            for (var i = 0; i < 20; i++)  flexLayout.Children.Add(new Label { Text = " " });

//            flexLayout.TabIndex = p.Index;

//            return flexLayout;
//        }

//        public View Build(OptimizedParagraph p, State state)
//        {
//            var flexLayout = new FlexLayout
//            {
//                Direction = FlexDirection.Row,
//                AlignItems = FlexAlignItems.End,
//                JustifyContent = FlexJustify.SpaceBetween,
//                Wrap = FlexWrap.Wrap,
//                Margin = new Thickness(0, 5, 0, 0)
//            };

//            var paragraphResult = state.GetCurrentSearchParagraphResult();

//            if (paragraphResult.Index == p.Index)
//            {
//                var pText = p.GetText();
//                if (string.IsNullOrWhiteSpace(state.SearchOptions.MainQuery))
//                {
//                    var aqWords = TextUtil.GetOrderedWords(state.SearchOptions.AdditionalQuery);
//                    var labels = GetLabels(pText, aqWords);
//                    foreach (var l in labels)
//                    {
//                        flexLayout.Children.Add(l);
//                    }
//                }
//                else
//                {
//                    var labels = GetLabels(pText, state.SearchOptions.MainQuery, state.SearchOptions.AdditionalQuery);
//                    foreach (var l in labels)
//                    {
//                        flexLayout.Children.Add(l);
//                    }
//                }
//            }
//            else
//            {
//                var words = p.GetWords().ToList();
//                var spaceSplit = TextUtil.GetSpaceSplit(words).ToList();
//                foreach (var fragment in spaceSplit)
//                {
//                    var label = new Label { Text = fragment, TextColor = Color.Black };
//                    flexLayout.Children.Add(label);
//                }
//            }

//            // you need to get your hands dirty when no option to justify
//            for (var i = 0; i < 20; i++)
//            {
//                flexLayout.Children.Add(new Label { Text = " "});
//            }

//            flexLayout.TabIndex = p.Index;

//            return flexLayout;
//        }

//        private IEnumerable<Label> GetLabels(string text, string mq, string aq)
//        {
//            mq = mq.ToLower();

//            var aqWords = TextUtil.GetOrderedWords(aq);

//            var lowerText = text.ToLower();

//            var mqIndex = lowerText.IndexOf(mq);

//            var beforeMq = mqIndex > 0 ? text.Substring(0, mqIndex) : null;

//            var concreteMq = text.Substring(mqIndex, mq.Length);

//            var afterMq = mqIndex + mq.Length <= text.Length - 1 ?  text.Substring(mqIndex + mq.Length) : null;

//            foreach (var l in GetLabels(beforeMq, aqWords))
//            {
//                yield return l;
//            }

//            yield return new Label { Text = $"<b>{concreteMq}</b> ", TextType = TextType.Html, TextColor = Color.Black };

//            foreach (var l in GetLabels(afterMq, aqWords))
//            {
//                yield return l;
//            }
//        }

//        private IEnumerable<Label> GetLabels(string text, List<string> aqWords)
//        {
//            if (!string.IsNullOrWhiteSpace(text))
//            {
//                var words = text.Split(' ');
//                foreach (var w in words)
//                {
//                    var aqMatch = aqWords.FirstOrDefault(aqw => w.ToLower().StartsWith(aqw));
//                    if (aqMatch != null)
//                    {
//                        var labelHtml = $"<b>{w.Substring(0, aqMatch.Length)}</b>{w.Substring(aqMatch.Length)} ";
//                        yield return new Label { Text = labelHtml, TextType = TextType.Html, TextColor = Color.Black };
//                    }
//                    else
//                    {
//                        yield return new Label { Text = $"{w} ", TextColor = Color.Black };
//                    }
//                }
//            }
//        }
//    }
//}