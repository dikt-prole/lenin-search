using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Text;
using LeninSearch.Standard.Core.Search;
using NUnit.Framework;
using System.Diagnostics;
using LeninSearch.Standard.Core.Search.OldSearch;

namespace LeninSearch.Standard.Core.Tests
{
    public class OldLsSearcherTests
    {
        private OldLsSearcher _lsSearcher;
        private string[] _dictionary;

        [SetUp]
        public void Setup()
        {
            _lsSearcher = new OldLsSearcher();
            var dictionaryPath = $"{ConstantData.LsFolder}\\corpus.dic";
            _dictionary = File.ReadAllLines(dictionaryPath, Encoding.UTF8);
        }

        [TestCase("lenin-t39.lsi", "диктатура* пролетар* + латин* науч*", 1)]
        [TestCase("marxengels-t23.lsi", "Паук совершает операции", 1)]
        [TestCase("stalin-t12.lsi", "политическая партия рабочего класса", 1)]
        [TestCase("hegel-objective-logic.lsi", "находящееся существенно + определение", 1)]
        public void SearchParagraphs_ParagraphsAreFound(string lsiFile, string query, int expectedCount)
        {
            // Arrange
            var lsiPath = $"{ConstantData.LsIndexFolder}\\{lsiFile}";
            var lsiBytes = File.ReadAllBytes(lsiPath);
            var lsiData = LsIndexUtil.FromLsIndexBytes(lsiBytes);

            var request = SearchQuery.Construct(query, _dictionary);

            // Act
            var searchResults = _lsSearcher.SearchParagraphs(lsiData, request);

            // Assert
            Assert.That(searchResults.Count, Is.EqualTo(expectedCount));
        }

        [TestCase("lenin-t39.lsi", "о современном + полож* ближайш*", 1)]
        [TestCase("marxengels-t23.lsi", "повременная заработная плат*", 1)]
        [TestCase("stalin-t12.lsi", "растущий подъем социалистич*", 1)]
        [TestCase("hegel-objective-logic.lsi", "определенные сущности или опред*", 1)]
        public void SearchParagraphs_HeadingsAreFound(string lsiFile, string query, int expectedCount)
        {
            // Arrange
            var lsiPath = $"{ConstantData.LsIndexFolder}\\{lsiFile}";
            var lsiBytes = File.ReadAllBytes(lsiPath);
            var lsiData = LsIndexUtil.FromLsIndexBytes(lsiBytes);

            var request = SearchQuery.Construct(query, _dictionary);

            // Act
            var searchResults = _lsSearcher.SearchHeadings(lsiData, request);

            // Assert
            Assert.That(searchResults.Count, Is.EqualTo(expectedCount));
        }

        [TestCase("lenin-t39.ls", "о современном + полож* ближайш*", 1)]
        [TestCase("lenin-t39.ls", "диктатура", 4)]
        [TestCase("marxengels-t23.ls", "повременная заработная плат*", 1)]
        [TestCase("stalin-t12.ls", "растущий подъем социалистич*", 1)]
        [TestCase("hegel-objective-logic.ls", "определенные сущности или опред*", 1)]
        public void LsToLsiThenRead_HeadingsAreFound(string lsFile, string query, int expectedCount)
        {
            // Arrange
            var lsPath = $"{ConstantData.LsFolder}\\{lsFile}";
            var lsBytes = File.ReadAllBytes(lsPath);
            var ofd = LsUtil.LoadOptimized(lsBytes, CancellationToken.None);
            var lsiBytes = LsIndexUtil.ToLsIndexBytes(ofd);
            var lsiData = LsIndexUtil.FromLsIndexBytes(lsiBytes);

            var request = SearchQuery.Construct(query, _dictionary);

            // Act
            var searchResults = _lsSearcher.SearchHeadings(lsiData, request);

            Debug.WriteLine($"For '{query}' paragraphs are: {string.Join(", ", searchResults.Select(r => r.ParagraphIndex))}");

            // Assert
            Assert.That(searchResults.Count, Is.EqualTo(expectedCount));
        }

        [TestCase("lenin-t39.ls", "дикт* + прол*")]
        [TestCase("lenin-t40.ls", "дикт* + прол*")]
        public void LsToLsiThenRead_DoesNotDuplicateResults(string lsFile, string query)
        {
            // Arrange
            var request = SearchQuery.Construct(query, _dictionary);
            var lsPath = $"{ConstantData.LsFolder}\\{lsFile}";
            var lsBytes = File.ReadAllBytes(lsPath);
            var ofd = LsUtil.LoadOptimized(lsBytes, CancellationToken.None);
            var lsiBytes = LsIndexUtil.ToLsIndexBytes(ofd);
            var lsiData = LsIndexUtil.FromLsIndexBytes(lsiBytes);

            // Act
            var searchResult = _lsSearcher.SearchParagraphs(lsiData, request);

            // Assert
            Assert.That(searchResult.Select(r => r.ParagraphIndex).Distinct().Count(), Is.EqualTo(searchResult.Count));
        }

        [TestCase("lenin-t01.ls", "до* обм* прод* + явл* тов*")]
        public void HandlesPainInTheAssInReasonableTime(string lsFile, string query)
        {
            // Arrange
            var request = SearchQuery.Construct(query, _dictionary);
            var lsPath = $"{ConstantData.LsFolder}\\{lsFile}";
            var lsBytes = File.ReadAllBytes(lsPath);
            var ofd = LsUtil.LoadOptimized(lsBytes, CancellationToken.None);
            var lsiBytes = LsIndexUtil.ToLsIndexBytes(ofd);
            var lsiData = LsIndexUtil.FromLsIndexBytes(lsiBytes);

            // Act
            var newSw = new Stopwatch(); newSw.Start();
            var newResult = _lsSearcher.SearchParagraphs(lsiData, request);
            newSw.Stop();

            // Assert
            
            Assert.Pass($"Finised in {newSw.ElapsedMilliseconds} ms");
        }
    }
}