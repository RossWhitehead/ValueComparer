using Xunit;

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
        public void AssertEqual_ThrowsNotEqualException_WhenAtLeastOneObjectIsNull(object object1, object object2)
        {
            // Assert
            Assert.Throws<EqualException>(() => ValueComparer.AssertEqual(object1, object2));
        }

        [Theory]
        [InlineData(1, 1.0)]
        public void Compare_ThrowsNotEqualException_WhenTypesOfObject1And2AreaNotEqual(object object1, object object2)
        {
            // Assert
            Assert.Throws<EqualException>(() => ValueComparer.AssertEqual(object1, object2));
        }

        [Theory]
        [InlineData(1, 1, true)]
        [InlineData(1, 2, false)]
        public void Compare_ExpectedResult_ForIComparableTypes(object object1, object object2, bool areEqual)
        {
            // Assert
            if (areEqual)
            {
                ValueComparer.AssertEqual(object1, object2);
            }
            else
            {
                Assert.Throws<EqualException>(() => ValueComparer.AssertEqual(object1, object2));
            }
        }

        [Fact]
        public void Compare_ThrowsEqualException_ForNotEqualClasses()
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

            // Assert
            Assert.Throws<EqualException>(() => ValueComparer.AssertEqual(fake1, fake2));
        }

        private class Fake
        {
            public int Prop1 { get; set; }
            public string Prop2 { get; set; }
            public Fake InnerFake { get; set; }
        }

    }
}
