namespace TonLibDotNet.Cells
{
    public class SliceTests
    {
        [Fact]
        public void ReadsOk()
        {
            var hex = "12345678";

            var slice = new Cell(Convert.FromHexString(hex), false).BeginRead();

            // Bits: 0001 0010 0011 0100 0101 0110 0111 1000

            Assert.False(slice.LoadBit());
            Assert.False(slice.LoadBit());

            // Bits: --01 0010 0011 0100 0101 0110 0111 1000

            Assert.Equal(0b01_0010_00, slice.LoadByte());

            // Bits: ---- ---- --11 0100 0101 0110 0111 1000

            Assert.Equal(new byte[] { 0b11_0100_01, 0b01_0110_01 }, slice.LoadBytes(2));

            // Bits: ---- ---- ---- ---- ---- ---- --11 1000

            Assert.Equal(new byte[] { 0b111 }, slice.LoadBitsToBytes(3));

            // Bits: ---- ---- ---- ---- ---- ---- ---- -000

            Assert.Equal(new byte[] { 0 }, slice.LoadBitsToBytes(3));

            slice.EndRead();
        }

        [Fact]
        public void CantReadMore()
        {
            var hex = "12345678";

            var cell = new Cell(Convert.FromHexString(hex), false);
            var slice = cell.BeginRead();

            _ = slice.LoadBytes(4);

            Assert.Throws<InvalidOperationException>(() => slice.LoadBit());
        }

        [Fact]
        public void EndReadThrows()
        {
            var hex = "12345678";

            var cell = new Cell(Convert.FromHexString(hex), false);
            var slice = cell.BeginRead();

            _ = slice.LoadBytes(1);

            Assert.Throws<InvalidOperationException>(() => slice.EndRead());
        }
    }
}
