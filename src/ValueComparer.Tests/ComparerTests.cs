using System.Collections.Generic;
using Xunit;
using Xunit.Extensions;

namespace ValueComparer
{
    public class ComparerTests
    {
        public ComparerTests()
        {
        }

        [Theory]
        [InlineData(1, null)]
        [InlineData(null, 2)]
        public void Compare_ReturnsFalse_WhenAtLeastOneObjectIsNull(object object1, object object2)
        {
            // Act
            var actualResult = ValueComparer.Compare(object1, object2);

            // Assert
            Assert.Equal(false, actualResult);
        }

        [Theory]
        [InlineData(1, 1.0)]
        public void Compare_ReturnsFalse_WhenTypesOfObject1And2AreaNotEqual(object object1, object object2)
        {
            // Act
            var actualResult = ValueComparer.Compare(object1, object2);

            // Assert
            Assert.Equal(false, actualResult);
        }

        [Theory]
        [InlineData(1, 1, true)]
        [InlineData(1, 2, false)]
        public void Compare_ReturnsExpectedResult_ForIComparableTypes(object object1, object object2, bool expectedResult)
        {
            // Act
            var actualResult = ValueComparer.Compare(object1, object2);

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void Compare_ReturnsTrue_ForObjectsOfSameTypeAndValues()
        {
            // Arrange
            var fake1 = new Fake()
            {
                Prop1 = 1,
                Prop2 = "string 1",
                InnerFake = new Fake(),
            };

            var fake2 = new Fake()
            {
                Prop1 = 1,
                Prop2 = "string 1",
                InnerFake = new Fake() { Prop1 = 2 }
            };

            // Act
            var actualResult = ValueComparer.Compare(fake1, fake2);

            // Assert
            Assert.Equal(false, actualResult);
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
                    new int[2] { 1, 2 },
                    new int[3] { 1, 2, 3 },
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
                }
            };
        }

        private class Fake
        {
            public int Prop1 { get; set; }
            public string Prop2 { get; set; }
            public Fake InnerFake { get; set; }
        }

    }
}
