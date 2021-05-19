using System;
using System.IO;
using System.Linq;
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

        [TestCase("lenin-t39.lsi", "���������* ��������* + �����* ����*", 1)]
        [TestCase("marxengels-t23.lsi", "���� ��������� ��������", 1)]
        [TestCase("stalin-t12.lsi", "������������ ������ �������� ������", 1)]
        [TestCase("hegel-objective-logic.lsi", "����������� ����������� + �����������", 1)]
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

        [TestCase("lenin-t39.lsi", "� ����������� + �����* �������*", 1)]
        [TestCase("marxengels-t23.lsi", "����������� ���������� ����*", 1)]
        [TestCase("stalin-t12.lsi", "�������� ������ �����������*", 1)]
        [TestCase("hegel-objective-logic.lsi", "������������ �������� ��� �����*", 1)]
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
    }
}