using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using LeninSearch.Standard.Core.Search;
using NUnit.Framework;

namespace LeninSearch.Standard.Core.Tests
{
    public class LsIndexDataTests
    {
        private const string DicFile = @"C:\Users\vbncmx\source\repos\lenin-search\corpus\ls\corpus";
        private const string LsIndexFolder = @"C:\Users\vbncmx\source\repos\lenin-search\corpus\ls_index";
        private const string LsFolder = @"C:\Users\vbncmx\source\repos\lenin-search\corpus\ls_test";
        private string[] _dictionary;

        [SetUp]
        public void Setup()
        {
            var dictionaryPath = $"{LsFolder}\\corpus.dic";
            _dictionary = File.ReadAllLines(dictionaryPath, Encoding.UTF8);
        }

        [TestCase("lenin-t39.lsi", 114)]
        [TestCase("marxengels-t23.lsi", 45)]
        [TestCase("stalin-t12.lsi", 26)]
        [TestCase("hegel-objective-logic.lsi", 203)]
        public void HeadingCountIsFine(string lsiFile, int headingCount)
        {
            // Arrange
            var lsiPath = $"{LsIndexFolder}\\{lsiFile}";
            var lsiBytes = File.ReadAllBytes(lsiPath);

            // Act
            var lsiData = LsIndexUtil.FromLsIndexBytes(lsiBytes);

            // Assert
            Assert.That(lsiData.HeadingData.Count, Is.EqualTo(headingCount));
        }

        [TestCase("lenin-t39.lsi", 487)]
        [TestCase("marxengels-t23.lsi", 885)]
        [TestCase("hegel-objective-logic.lsi", 510)]
        public void PageCountIsFine(string lsiFile, int pageCount)
        {
            // Arrange
            var lsiPath = $"{LsIndexFolder}\\{lsiFile}";
            var lsiBytes = File.ReadAllBytes(lsiPath);

            // Act
            var lsiData = LsIndexUtil.FromLsIndexBytes(lsiBytes);

            // Assert
            Assert.That(lsiData.PageData.Count, Is.EqualTo(pageCount));
            Assert.That(lsiData.PageData.All(pd => pd.Number < 1000));
        }

        [TestCase("lenin-t39.lsi", 100)]
        [TestCase("marxengels-t23.lsi", 100)]
        [TestCase("stalin-t12.lsi", 100)]
        [TestCase("hegel-objective-logic.lsi", 100)]
        public void ToLsDataWorksInReasonableTime(string lsiFile, int timeMs)
        {
            // Arrange
            var lsiPath = $"{LsIndexFolder}\\{lsiFile}";
            var lsiBytes = File.ReadAllBytes(lsiPath);
            var lsiData = LsIndexUtil.FromLsIndexBytes(lsiBytes);

            // Act
            var sw = new Stopwatch();
            sw.Start();
            var lsData = lsiData.ToLsData();
            sw.Stop();

            // Assert
            Assert.That(sw.ElapsedMilliseconds, Is.LessThan(timeMs));
        }

        [TestCase("lenin-t39.lsi", (ushort)1870, "мы не можем их даже приблизительно сравнивать")]
        [TestCase("marxengels-t23.lsi", (ushort)1818, "нечто отличное от его собственной потребительной стоимости")]
        [TestCase("stalin-t12.lsi", (ushort)1924, "В этом же основа")]
        [TestCase("hegel-objective-logic.lsi", (ushort)1770, "способ проявления указанного единства")]
        public void ToLsDataProducesReasonableText(string lsiFile, ushort paragraphIndex, string stringToken)
        {
            // Arrange
            var lsiPath = $"{LsIndexFolder}\\{lsiFile}";
            var lsiBytes = File.ReadAllBytes(lsiPath);
            var lsiData = LsIndexUtil.FromLsIndexBytes(lsiBytes);

            // Act
            var lsData = lsiData.ToLsData();

            // Assert
            var paragraphText = lsData.Paragraphs[paragraphIndex].GetText(_dictionary);
            Assert.That(paragraphText, Contains.Substring(stringToken));
        }
    }
}