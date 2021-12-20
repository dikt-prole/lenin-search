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
        private string[] _dictionary;

        [SetUp]
        public void Setup()
        {
            var dictionaryPath = $"{ConstantData.LsFolder}\\corpus.dic";
            _dictionary = File.ReadAllLines(dictionaryPath, Encoding.UTF8);
        }

        [TestCase("D:\\Repo\\lenin-search\\corpus\\ls_index\\main\\v2\\lenin-t39.lsi", 114)]
        [TestCase("D:\\Repo\\lenin-search\\corpus\\ls_index\\main\\v2\\marx-engels-t23.lsi", 45)]
        [TestCase("D:\\Repo\\lenin-search\\corpus\\ls_index\\main\\v2\\stalin-t12.lsi", 26)]
        [TestCase("D:\\Repo\\lenin-search\\corpus\\ls_index\\main\\v2\\hegel-objective-logic.lsi", 203)]
        [TestCase("D:\\Repo\\lenin-search\\corpus\\ls_index\\klgd\\v1\\yt0j8q2xiULRM.lsi", 0)]
        public void HeadingCountIsFine(string lsiFile, int headingCount)
        {
            // Arrange
            var lsiBytes = File.ReadAllBytes(lsiFile);

            // Act
            var lsiData = LsIndexUtil.FromLsIndexBytes(lsiBytes);

            // Assert
            Assert.That(lsiData.HeadingData.Count, Is.EqualTo(headingCount));
        }

        [TestCase("D:\\Repo\\lenin-search\\corpus\\ls_index\\main\\v2\\lenin-t39.lsi", 487)]
        [TestCase("D:\\Repo\\lenin-search\\corpus\\ls_index\\main\\v2\\marx-engels-t23.lsi", 885)]
        [TestCase("D:\\Repo\\lenin-search\\corpus\\ls_index\\main\\v2\\hegel-objective-logic.lsi", 510)]
        [TestCase("D:\\Repo\\lenin-search\\corpus\\ls_index\\klgd\\v1\\yt0j8q2xiULRM.lsi", 0)]
        public void PageCountIsFine(string lsiFile, int pageCount)
        {
            // Arrange
            var lsiBytes = File.ReadAllBytes(lsiFile);

            // Act
            var lsiData = LsIndexUtil.FromLsIndexBytes(lsiBytes);

            // Assert
            Assert.That(lsiData.PageData.Count, Is.EqualTo(pageCount));
            Assert.That(lsiData.PageData.All(pd => pd.Number < 1000));
        }

        [TestCase("D:\\Repo\\lenin-search\\corpus\\ls_index\\main\\v2\\lenin-t39.lsi", 200)]
        [TestCase("D:\\Repo\\lenin-search\\corpus\\ls_index\\main\\v2\\marx-engels-t23.lsi", 200)]
        [TestCase("D:\\Repo\\lenin-search\\corpus\\ls_index\\main\\v2\\stalin-t12.lsi", 200)]
        [TestCase("D:\\Repo\\lenin-search\\corpus\\ls_index\\main\\v2\\hegel-objective-logic.lsi", 200)]
        [TestCase("D:\\Repo\\lenin-search\\corpus\\ls_index\\klgd\\v1\\yt0j8q2xiULRM.lsi", 200)]
        public void ToLsDataWorksInReasonableTime(string lsiFile, int timeMs)
        {
            // Arrange
            var lsiBytes = File.ReadAllBytes(lsiFile);
            var lsiData = LsIndexUtil.FromLsIndexBytes(lsiBytes);

            // Act
            var sw = new Stopwatch();
            sw.Start();
            var lsData = lsiData.ToLsData();
            sw.Stop();

            // Assert
            Assert.That(sw.ElapsedMilliseconds, Is.LessThan(timeMs));
        }

        [TestCase("D:\\Repo\\lenin-search\\corpus\\ls_index\\main\\v2\\lenin-t39.lsi", (ushort)1870, "мы не можем их даже приблизительно сравнивать")]        
        [TestCase("D:\\Repo\\lenin-search\\corpus\\ls_index\\main\\v2\\marx-engels-t23.lsi", (ushort)1818, "нечто отличное от его собственной потребительной стоимости")]
        [TestCase("D:\\Repo\\lenin-search\\corpus\\ls_index\\main\\v2\\stalin-t12.lsi", (ushort)1924, "В этом же основа")]
        [TestCase("D:\\Repo\\lenin-search\\corpus\\ls_index\\main\\v2\\hegel-objective-logic.lsi", (ushort)1770, "способ проявления указанного единства")]
        public void ToLsDataProducesReasonableText(string lsiFile, ushort paragraphIndex, string stringToken)
        {
            // Arrange
            var lsiBytes = File.ReadAllBytes(lsiFile);
            var lsiData = LsIndexUtil.FromLsIndexBytes(lsiBytes);

            // Act
            var lsData = lsiData.ToLsData();

            // Assert
            var paragraphText = lsData.Paragraphs[paragraphIndex].GetText(_dictionary);
            Assert.That(paragraphText, Contains.Substring(stringToken));
        }
    }
}