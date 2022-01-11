using System;
using System.Linq;
using LeninSearch.Standard.Core.Corpus.Lsi;
using Xamarin.Forms;

namespace LeninSearch.Xam
{
    public static class ViewFactory
    {
        public static View ConstructHeadingView(LsiHeadingLeaf headingLeaf, string[] dictionaryWords, Action<LsiHeadingLeaf> readAction)
        {
            var verticalStack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Margin = new Thickness(0, 10, 0, 0)
            };

            var horizontalStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            verticalStack.Children.Add(horizontalStack);
            var childViews = headingLeaf.Children.Select(l => ConstructHeadingView(l, dictionaryWords, readAction)).ToList();
            foreach (var childView in childViews)
            {
                childView.Margin = new Thickness(10, 10, 0, 0);
                childView.IsVisible = false;
                verticalStack.Children.Add(childView);
            }

            var bulletButton = new ImageButton
            {
                Source = "bullet.png",
                HeightRequest = 24,
                WidthRequest = 24,
                Margin = 0,
                HorizontalOptions = LayoutOptions.Start,
                BackgroundColor = Color.White,
                Padding = 5
            };
            bulletButton.Clicked += (sender, args) => readAction(headingLeaf);
            horizontalStack.Children.Add(bulletButton);

            var label = new Label
            {
                FontSize = Settings.UI.Font.SmallFontSize,
                TextColor = Color.Black,
                Text = headingLeaf.GetText(dictionaryWords),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Margin = new Thickness(0, 3, 0, 0)
            };
            horizontalStack.Children.Add(label);

            if (headingLeaf.Children.Any())
            {
                var expandCollapseButton = new ImageButton
                {
                    Source = "expand.png",
                    HeightRequest = 24,
                    WidthRequest = 24,
                    Margin = 0,
                    HorizontalOptions = LayoutOptions.End,
                    BackgroundColor = Color.White,
                    Padding = 5
                };
                var isExpanded = false;
                expandCollapseButton.Clicked += (sender, args) =>
                {
                    isExpanded = !isExpanded;
                    foreach (var childView in childViews)
                    {
                        childView.IsVisible = isExpanded;
                    }

                    expandCollapseButton.Source = isExpanded ? "collapse.png" : "expand.png";
                };
                horizontalStack.Children.Add(expandCollapseButton);
            }

            return verticalStack;
        }
    }
}