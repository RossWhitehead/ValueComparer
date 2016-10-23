using System.Collections.Generic;
using Xunit;
using Xunit.Extensions;

namespace ValueComparer
{
    public class ComparerCollectionTests
    {
        public ComparerCollectionTests()
        {
        }

        [Theory]
        [MemberData(("CollectionsData"), DisableDiscoveryEnumeration = true)]
        public void Compare_ReturnsExpectedResult_ForCollections(object object1, object object2, bool expectedResult)
        {
            // Act
            var actualResult = ValueComparer.Compare(object1, object2);

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        public static IEnumerable<object[]> CollectionsData()
        {
            return new[]
            {
                new object[]
                {
                    new int[2] { 1, 2 },
                    new int[2] { 1, 2 },
                    true
                },
                new object[]
                {
                    new int[2] { 1, 2 },
                    new int[2] { 1, 3 },
                    false
                },
                new object[]
                {
                    new string[2] { "one", "two" },
                    new string[3] { "one", "two", "three" },
                    false
                },
                // The MemberData attribute needs DisableDiscoveryEnumeration = true for the following to work
                new object[]
                {
                    new int[2, 2] { { 1, 2 }, { 3, 4 } },
                    new int[2, 2] { { 1, 2 }, { 3, 4 } },
                    true
                },
                new object[]
                {
                    new int[2, 2] { { 1, 2 }, { 3, 4 } },
                    new int[2, 2] { { 1, 6 }, { 3, 4 } },
                    false
                },
                new object[]
                {
                    new byte[2][] { new byte[2] { 1, 2 }, new byte[3] { 3, 4, 5 } },
                    new byte[2][] { new byte[2] { 1, 2 }, new byte[3] { 3, 4, 5 } },
                    true
                },
                new object[]
                {
                    new byte[2][] { new byte[2] { 1, 2 }, new byte[3] { 3, 4, 5 } },
                    new byte[2][] { new byte[2] { 1, 2 }, new byte[3] { 3, 4, 6 } },
                    false
                }
            };
        }
    }
}
