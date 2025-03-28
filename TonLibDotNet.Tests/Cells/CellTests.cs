﻿namespace TonLibDotNet.Cells
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

        [Fact]
        public void HashOk()
        {
            // sample data from TestBOCBomb at https://github.com/xssnick/tonutils-go/blob/619c2aa1f6b992997bf322f8f9bfc4ae036a5181/tvm/cell/cell_test.go#L206-L215
            var boc_base64 = "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";
            var hash_hex = "EC89185485A679715A615BCA264BF01B4CB619BD28D9E17FF73D05CF5E01C0E0";

            var boc = Boc.ParseFromBase64(boc_base64);
            var rootCell = boc.RootCells[0];
            var hash = rootCell.Hash();

            Assert.Equal(hash_hex, Convert.ToHexString(hash).ToUpperInvariant());
        }
    }
}
