using System.Numerics;

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
            slice.EndRead();
        }

        [Fact]
        public void WritesAddressesOk()
        {
            var cell = new CellBuilder()
                .StoreAddressIntStd("EQCQDn3i2nlXFZhQ5EG_QdONTdHdtNxhHjPYb3lR1eG9xTTb")
                .Build();

            var slice = cell.BeginRead();
            Assert.Equal("EQCQDn3i2nlXFZhQ5EG_QdONTdHdtNxhHjPYb3lR1eG9xTTb", slice.LoadAddressIntStd());
            slice.EndRead();
        }

        [Fact]
        public void WritesCoinsOk()
        {
            var builder = new CellBuilder()
                .StoreCoins(123456789)
                .StoreCoins(BigInteger.Zero)
                .StoreCoins(1_000_000_000__000_000_000) // 1 G TON
                .StoreCoins(1_000_000_001)
                .StoreCoins(1_000_000_002)
                .StoreCoins(new BigInteger(1_000_000_003))
                ;

            var slice = builder.Build().BeginRead();

            Assert.Equal(123456789, slice.LoadCoins());
            Assert.Equal(0, slice.LoadCoins());
            Assert.Equal(1_000_000_000__000_000_000, slice.LoadCoins());

            // read long as ulong
            Assert.Equal((ulong)1_000_000_001, slice.LoadCoinsToULong());

            // read long ad bigint
            Assert.Equal(new BigInteger(1_000_000_002), slice.LoadCoinsToBigInt());

            // read bigint as long
            Assert.Equal(1_000_000_003, slice.LoadCoins());

            slice.EndRead();
        }

        [Fact]
        public void WritesCoins2Ok()
        {
            var builder = new CellBuilder();

            // negative values are not allowed
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.StoreCoins(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.StoreCoins(BigInteger.MinusOne));

            // more than 120 bits are not allowed
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.StoreCoins(BigInteger.Pow(2, 121)));


            // greather than long
            var veryBig = new BigInteger(long.MaxValue) * 19;
            builder.StoreCoins(veryBig);

            var slice = builder.Build().BeginRead();

            // can't load to long
            Assert.Throws<InvalidOperationException>(() => slice.LoadCoins());

            // but can load to bigint
            var veryBig2 = slice.LoadCoinsToBigInt();
            Assert.Equal(veryBig, veryBig2);

            slice.EndRead();
        }
    }
}
