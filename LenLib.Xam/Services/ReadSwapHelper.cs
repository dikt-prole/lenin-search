using System;
using System.Collections.Generic;
using System.Linq;
using LenLib.Standard.Core.Corpus.Lsi;
using LenLib.Standard.Core.Search;
using LenLib.Xam.ListItems;
using Xamarin.Forms.Internals;

namespace LenLib.Xam.Services
{
    public class ReadSwapHelper
    {
        public const int ReadSwapCount = 20;

        private readonly ILsiProvider _lsiProvider;
        private readonly Action<string> _alertAction;
        private readonly Func<ushort> _imageWidthFunc;
        private readonly Action<ReadListItem> _tappedAction;

        public ReadSwapHelper(
            ILsiProvider lsiProvider, 
            Action<string> alertAction, 
            Func<ushort> imageWidthFunc, 
            Action<ReadListItem> tappedAction)
        {
            _lsiProvider = lsiProvider;
            _alertAction = alertAction;
            _imageWidthFunc = imageWidthFunc;
            _tappedAction = tappedAction;
        }

        public List<ReadListItem> GetInitialReadSwap(string corpusId, string file, ushort selectedParagraphIndex, SearchUnit searchUnit = null)
        {
            var lsiData = _lsiProvider.GetLsiData(corpusId, file);
            var paragraphIndexes = lsiData.Paragraphs.Keys.OrderBy(k => k).ToArray();
            var middleIndex = paragraphIndexes.IndexOf(selectedParagraphIndex);

            var startIndex = middleIndex - ReadSwapCount;
            if (startIndex < 0)
            {
                startIndex = 0;
            }

            var endIndex = middleIndex + ReadSwapCount;
            if (endIndex > paragraphIndexes.Length - 1)
            {
                endIndex = paragraphIndexes.Length - 1;
            }

            var swapIndexes = new List<ushort>();
            for (var i = startIndex; i <= endIndex; i++)
            {
                swapIndexes.Add(paragraphIndexes[i]);
            }

            return ConstructReadSwap(lsiData, corpusId, file, swapIndexes.ToArray(), searchUnit);
        }

        public List<ReadListItem> GetNextReadSwap(string corpusId, string file, ushort lastParagraphIndex)
        {
            var lsiData = _lsiProvider.GetLsiData(corpusId, file);
            var paragraphIndexes = lsiData.Paragraphs.Keys.OrderBy(k => k).ToArray();

            var startIndex = paragraphIndexes.IndexOf(lastParagraphIndex) + 1;
            if (startIndex > paragraphIndexes.Length - 1)
            {
                return new List<ReadListItem>();
            }

            var endIndex = startIndex + ReadSwapCount;
            if (endIndex > paragraphIndexes.Length - 1)
            {
                endIndex = paragraphIndexes.Length - 1;
            }

            var swapIndexes = new List<ushort>();
            for (var i = startIndex; i <= endIndex; i++)
            {
                swapIndexes.Add(paragraphIndexes[i]);
            }

            return ConstructReadSwap(lsiData, corpusId, file, swapIndexes.ToArray());
        }

        public List<ReadListItem> GetPrevReadSwap(string corpusId, string file, ushort firstParagraphIndex)
        {
            var lsiData = _lsiProvider.GetLsiData(corpusId, file);
            var paragraphIndexes = lsiData.Paragraphs.Keys.OrderBy(k => k).ToArray();

            var endIndex = paragraphIndexes.IndexOf(firstParagraphIndex) - 1;
            if (endIndex < 0)
            {
                return new List<ReadListItem>();
            }

            var startIndex = endIndex - ReadSwapCount;
            if (startIndex < 0)
            {
                startIndex = 0;
            }

            var swapIndexes = new List<ushort>();
            for (var i = startIndex; i <= endIndex; i++)
            {
                swapIndexes.Add(paragraphIndexes[i]);
            }

            return ConstructReadSwap(lsiData, corpusId, file, swapIndexes.ToArray());
        }

        private List<ReadListItem> ConstructReadSwap(LsiData lsiData, string corpusId, string file, ushort[] swapIndexes, SearchUnit searchUnit = null)
        {
            var readListItems = new List<ReadListItem>();
            foreach (var paragraphIndex in swapIndexes)
            {
                var lsiParagraph = lsiData.Paragraphs[paragraphIndex];
                var readListItem = ReadListItem.Construct(
                    corpusId, 
                    file,
                    lsiParagraph, 
                    _lsiProvider, 
                    searchUnit,
                    _imageWidthFunc,
                    _alertAction,
                    _tappedAction);
                readListItem.FontSize = Settings.Instance.FontSize;
                readListItems.Add(readListItem);
            }

            return readListItems;
        }
    }
}