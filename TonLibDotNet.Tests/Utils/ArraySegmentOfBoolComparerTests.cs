namespace TonLibDotNet.Utils
{
    public class ArraySegmentOfBoolComparerTests
    {
        [Fact]
        public void EmptyAreEqual()
        {
            Assert.Equal(0, ArraySegmentOfBoolComparer.Instance.Compare(new bool[0], ArraySegment<bool>.Empty));
        }

        [Theory]
        // Same length tests
        [InlineData("10101", "10101", 0)]
        [InlineData("10001", "10101", -1)]
        [InlineData("10001", "00101", 1)]
        // Now different length, but significant bits in "common" part
        [InlineData("101", "11111", -1)]
        [InlineData("11110000", "100", 1)]
        // Now different length, and "common" parts are equal, so longer sequence is "greater"
        [InlineData("1010", "10101010", -1)]
        public void Test(string left, string right, int expected)
        {
            var x = left.Select(x => x == '1').ToArray();
            var y = right.Select(x => x == '1').ToArray();
            Assert.Equal(expected, ArraySegmentOfBoolComparer.Instance.Compare(x, y));
        }
    }
}
