using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Text;
using LeninSearch.Standard.Core.Search;
using NUnit.Framework;
using System.Diagnostics;

namespace LeninSearch.Standard.Core.Tests
{
    public class LsSearcherTests
    {
        private LsSearcher _lsSearcher;
        private const string LsIndexFolder = @"E:\Repo\lenin-search\corpus\ls_index";
        private const string LsFolder = @"E:\Repo\lenin-search\corpus\ls_test";
        private string[] _dictionary;

        [SetUp]
        public void Setup()
        {
            _lsSearcher = new LsSearcher();
            var dictionaryPath = $"{LsFolder}\\corpus.dic";
            _dictionary = File.ReadAllLines(dictionaryPath, Encoding.UTF8);
        }

        [TestCase("lenin-t39.lsi", "диктатура* пролетар* + латин* науч*", 1)]
        [TestCase("marxengels-t23.lsi", "Паук совершает операции", 1)]
        [TestCase("stalin-t12.lsi", "политическая партия рабочего класса", 1)]
        [TestCase("hegel-objective-logic.lsi", "находящееся существенно + определение", 1)]
        public void SearchParagraphs_ParagraphsAreFound(string lsiFile, string query, int expectedCount)
        {
            // Arrange
            var lsiPath = $"{LsIndexFolder}\\{lsiFile}";
            var lsiBytes = File.ReadAllBytes(lsiPath);
            var lsiData = LsIndexUtil.FromLsIndexBytes(lsiBytes);

            var request = SearchRequest.Construct(query, _dictionary);

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
            var lsiPath = $"{LsIndexFolder}\\{lsiFile}";
            var lsiBytes = File.ReadAllBytes(lsiPath);
            var lsiData = LsIndexUtil.FromLsIndexBytes(lsiBytes);

            var request = SearchRequest.Construct(query, _dictionary);

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
            var lsPath = $"{LsFolder}\\{lsFile}";
            var lsBytes = File.ReadAllBytes(lsPath);
            var ofd = LsUtil.LoadOptimized(lsBytes, CancellationToken.None);
            var lsiBytes = LsIndexUtil.ToLsIndexBytes(ofd);
            var lsiData = LsIndexUtil.FromLsIndexBytes(lsiBytes);

            var request = SearchRequest.Construct(query, _dictionary);

            // Act
            var searchResults = _lsSearcher.SearchHeadings(lsiData, request);

            Debug.WriteLine($"For '{query}' paragraphs are: {string.Join(", ", searchResults.Select(r => r.ParagraphIndex))}");

            // Assert
            Assert.That(searchResults.Count, Is.EqualTo(expectedCount));
        }
    }
}