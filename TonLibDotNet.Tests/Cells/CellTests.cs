namespace TonLibDotNet.Cells
{
    public class CellTests
    {
        [Theory]
        [InlineData("A01F75C0CC", false, 40)]
        [InlineData("A01F75C0CC", true, 40 - 3)] // last CC is '1100_1100', trimming last '100'
        [InlineData("A01F75C0C0", true, 40 - 7)] // last C0 is '1100_0000', trimming last '100_0000'
        public void BitsCountOk(string hexData, bool augmented, int validLength)
        {
            var cell = new Cell(Convert.FromHexString(hexData), augmented);
            Assert.Equal(validLength, cell.BitsCount);
        }

        [Theory]
        [InlineData("12345678", false, "0001 0010 0011 0100 0101 0110 0111 1000")]
        [InlineData("12345678", true, "0001 0010 0011 0100 0101 0110 0111")]
        public void BeginReadOk(string hexData, bool augmented, string expectedBinData)
        {
            var cell = new Cell(Convert.FromHexString(hexData), augmented);
            var slice = cell.BeginRead();

            var expected = expectedBinData.ToCharArray().Where(x => x != ' ').Select(x => x == '1').ToArray();
            var actual = Enumerable.Range(0, slice.Length).Select(x => slice.LoadBit()).ToArray();

            Assert.Equal(expected, actual);
        }
    }
}
