using System.IO;
using System.Net;
using System.Text;
using LeninSearch.Standard.Core.Search;
using NUnit.Framework;

namespace LeninSearch.Standard.Core.Tests
{
    public class LsSearcherTests
    {
        private LsSearcher _lsSearcher;
        private const string DicFile = @"C:\Users\vbncmx\source\repos\lenin-search\corpus\ls\corpus";
        private const string LsIndexFolder = @"C:\Users\vbncmx\source\repos\lenin-search\corpus\ls_index";
        private const string LsFolder = @"C:\Users\vbncmx\source\repos\lenin-search\corpus\ls_test";
        private string[] _dictionary;

        [SetUp]
        public void Setup()
        {
            _lsSearcher = new LsSearcher();
            var dictionaryPath = $"{LsFolder}\\corpus.dic";
            _dictionary = File.ReadAllLines(dictionaryPath, Encoding.UTF8);
        }

        [TestCase("lenin-t39.lsi", "диктатура* пролетар* + латин* науч*", 1)]
        public void SearchParagraphs_ParagraphsAreFound(string lsiFile, string query, int expectedCount)
        {
            // Arrange
            var lsiPath = $"{LsIndexFolder}\\{lsiFile}";
            var lsiBytes = File.ReadAllBytes(lsiPath);
            var lsiData = LsIndexUtil.FromLsIndexBytes(lsiBytes);

            var request = SearchRequest.Construct(query, _dictionary);

            // Act
            var result = _lsSearcher.SearchParagraphs(lsiData, request);

            // Assert
            Assert.That(result.Count, Is.EqualTo(expectedCount));
        }
    }
}