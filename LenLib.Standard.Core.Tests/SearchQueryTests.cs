using System.Diagnostics;
using System.IO;
using System.Text;
using LenLib.Standard.Core.Search;
using NUnit.Framework;

namespace LenLib.Standard.Core.Tests
{
    public class SearchQueryTests
    {
        private string[] _dictionary;

        [SetUp]
        public void Setup()
        {
            var dictionaryPath = $"{ConstantData.LsFolder}\\corpus.dic";
            _dictionary = File.ReadAllLines(dictionaryPath, Encoding.UTF8);
        }

        [TestCase("+ *123")]
        public void WorksInAReasonableTime(string query)
        {
            //Arrange
            //Act
            var sw = new Stopwatch(); sw.Start();
            var searchQuery = SearchQuery.Construct(query, _dictionary);
            sw.Stop();
            

            // Assert
            Assert.Pass($"Finised in {sw.ElapsedMilliseconds} ms");
        }
    }
}