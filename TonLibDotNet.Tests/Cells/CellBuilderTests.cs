namespace TonLibDotNet.Cells
{
    public class CellBuilderTests
    {
        [Fact]
        public void WritesOk()
        {
            var cell = new CellBuilder()
                .StoreBit(false)
                .StoreBit(false)
                .StoreByte(0b0100_1000)
                .StoreBytes(new byte[] { 0b11_0100_01, 0b01_0110_01 })
                .StoreBytes(new byte[] { 0b000_0110 }, 3)
                .Build();

            // Bits: 0001 0010 0011 0100 0101 0110 0111 0___
            // Bits: 0001 0010 0011 0100 0101 0110 0111 0100 - augmented
            Assert.True(cell.IsAugmented);
            Assert.Equal(29, cell.BitsCount);
            Assert.Equal("12345674", Convert.ToHexString(cell.Content));
        }

        [Fact]
        public void WritesNumericsOk()
        {
            var cell = new CellBuilder()
                .StoreUInt(77)
                .StoreUInt(77, 10)
                .StoreInt(123456789)
                .StoreUShort(77)
                .StoreUShort(77, 10)
                .StoreShort(0x7F0F)
                .Build();

            var slice = cell.BeginRead();
            Assert.Equal(77, (int)slice.LoadUInt());
            Assert.Equal(77, (int)slice.LoadUInt(10));
            Assert.Equal(123456789, slice.LoadInt());
            Assert.Equal(77, slice.LoadUShort());
            Assert.Equal(77, slice.LoadUShort(10));
            Assert.Equal(0x7F0F, slice.LoadShort());
        }

        [Fact]
        public void WritesAddressesOk()
        {
            var cell = new CellBuilder()
                .StoreAddressIntStd("EQCQDn3i2nlXFZhQ5EG_QdONTdHdtNxhHjPYb3lR1eG9xTTb")
                .Build();

            var slice = cell.BeginRead();
            Assert.Equal("EQCQDn3i2nlXFZhQ5EG_QdONTdHdtNxhHjPYb3lR1eG9xTTb", slice.LoadAddressIntStd());
        }

        [Fact]
        public void WritesGramsOk()
        {
            var cell = new CellBuilder()
                .StoreGrams(123456789)
                .StoreGrams(0)
                .StoreGrams(1_000_000_000__000_000_000) // 1 G TON
                .Build();

            var slice = cell.BeginRead();
            Assert.Equal(123456789, (long)slice.LoadGrams());
            Assert.Equal(0, (long)slice.LoadGrams());
            Assert.Equal(1_000_000_000__000_000_000, (long)slice.LoadGrams());
        }
    }
}
