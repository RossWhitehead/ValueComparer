using System.Collections.Generic;
using Xunit;

namespace ValueComparer
{
    public class ComparerDictionaryTests
    {
        public ComparerDictionaryTests()
        {
        }

        [Theory]
        [MemberData("DictionariesData")]
        public void AssertEqual_ExpectedResult_ForDictionaries(object object1, object object2, bool areEqual)
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

        public static IEnumerable<object[]> DictionariesData()
        {
            return new[]
            {
                new object[]
                {
                    new Dictionary<int, string>() { { 1, "one" }, { 2, "two" }, { 3, "three" } },
                    new Dictionary<int, string>() { { 1, "one" }, { 2, "two" }, { 3, "three" } },
                    true
                },
                 new object[]
                {
                    new Dictionary<int, string>() { { 1, "one" }, { 2, "two" }, { 3, "three" } },
                    new Dictionary<int, string>() { { 1, "one" }, { 2, "two" }, { 4, "three" } },
                    false
                },
                  new object[]
                {
                    new Dictionary<int, string>() { { 1, "one" }, { 2, "two" }, { 3, "three" } },
                    new Dictionary<int, string>() { { 1, "one" }, { 2, "two" }, { 3, "four" } },
                    false
                }
            };
        }
    }
}
