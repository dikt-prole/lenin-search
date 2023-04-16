using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LenLib.Standard.Core.Search;
using NUnit.Framework;

namespace LenLib.Standard.Core.Tests
{
    public class IntersectionUtilTests
    {
        private static Random Random = new Random();

        [Test]
        public void WorksFineWith2Lists()
        {
            //Arrange
            var list1 = new List<ushort> {1, 3, 29, 3000, 7910, 11456, 25346};
            var list2 = new List<ushort> { 3, 75, 3000, 5981, 7612, 11456, 25346, 51876 };
            var orderedLists = new List<List<ushort>> {list1, list2};

            //Act
            var result = IntersectionUtil.IntersectOrderedLists(orderedLists).ToList();

            //Assert
            var expectedList = new List<ushort> {3, 3000, 11456, 25346};
            Assert.That(result, Is.EquivalentTo(expectedList));
        }

        [Test]
        public void WorksFasterThenStandardIntersect()
        {
            //Arrange
            var lists = new List<List<ushort>>();
            for (var c = 0; c < 6; c++)
            {
                var list = Enumerable.Range(0, 20000).Select(i => (ushort) Random.Next(ushort.MaxValue))
                    .Distinct().OrderBy(i => i).ToList();
                lists.Add(list);
            }

            //Act
            var swOrdered = new Stopwatch(); swOrdered.Start();
            var orderedResult = IntersectionUtil.IntersectOrderedLists(lists).ToList();
            swOrdered.Stop();

            var swStandard = new Stopwatch(); swStandard.Start();
            var standardResult = IntersectionUtil.IntersectStd(lists).ToList();
            swStandard.Stop();

            //Assert
            
            Assert.That(orderedResult, Is.EquivalentTo(standardResult));
            Assert.That(swOrdered.ElapsedMilliseconds, Is.LessThan(swStandard.ElapsedMilliseconds));
        }
    }
}