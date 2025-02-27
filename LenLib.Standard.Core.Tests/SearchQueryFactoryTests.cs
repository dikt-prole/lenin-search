﻿using System.Diagnostics;
using System.Linq;
using LenLib.Standard.Core.Search;
using NUnit.Framework;

namespace LenLib.Standard.Core.Tests
{
    public class SearchQueryFactoryTests
    {
        [Test]
        public void WorksFine()
        {
            // Arrange
            var tokenVariantProvider = new PorterTokenVariantProvider();
            var searchQueryFactory = new SearchQueryFactory(tokenVariantProvider);
            var dictionary = new string[] { };
            var queryText = "национальными вопросами";

            // Act
            var queries = searchQueryFactory.Construct(queryText, dictionary).OrderBy(q => q.Priority).ToList();

            // Assert
            foreach (var searchQuery in queries)
            {
                Debug.WriteLine($"{searchQuery.Text} = {searchQuery.Priority}");
            }

            Debug.WriteLine($"Total number of queries: {queries.Count}");
        }
    }
}