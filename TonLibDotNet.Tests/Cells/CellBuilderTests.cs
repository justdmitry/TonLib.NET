using System.Numerics;
using Xunit.Abstractions;

namespace TonLibDotNet.Cells
{
    public class CellBuilderTests
    {
        private readonly ITestOutputHelper output;

        public CellBuilderTests(ITestOutputHelper output)
        {
            this.output = output;
        }

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

        [Fact]
        public void WritesStringsOk()
        {
            var builder = new CellBuilder();

            var cell = builder.StoreString("Hello ").StoreString("World!").StoreString(" 😀").Build();

            var slice = cell.BeginRead();
            var text = slice.LoadString();

            Assert.Equal("Hello World! 😀", text);

            slice.EndRead();
        }


        [Fact]
        public void WritesEmptyStringsOk()
        {
            var builder = new CellBuilder();

            var cell = builder.StoreString(string.Empty).Build();

            Assert.Empty(cell.Content);

            var slice = cell.BeginRead();
            var text = slice.LoadString();

            Assert.Null(text);

            slice.EndRead();
        }

        [Fact]
        public void LoadStringFailsOnWrongBitsCount()
        {
            var builder = new CellBuilder();

            var cell = builder.StoreString("Hello, World!").Build();

            var slice = cell.BeginRead();

            slice.SkipBits(3);
            Assert.Throws<ArgumentOutOfRangeException>(() => slice.LoadString());
        }

        [Fact]
        public void WritesStringSnakeOk()
        {
            var text = @"Storing a very long text string, expecting it to split to several cell, chained one after another.
One cell can store up to 128 bytes, so lets make this text so it uses at least three cells to store.
This text has length of 283 character now, so this should be enough for our test.";

            var builder = new CellBuilder();

            var cell = builder.StoreStringSnake(text).Build();

            output.WriteLine(cell.ToBoc().DumpCells());

            var slice = cell.BeginRead();
            var text2 = slice.LoadStringSnake();

            Assert.Equal(text, text2);

            slice.EndRead();
        }

        [Theory]
        // BOC from random NFT collection
        [InlineData("b5ee9c7201010101002900004e0168747470733a2f2f6e66742e746f6e2e6469616d6f6e64732f6469616d6f6e64732e6a736f6e", "https://nft.ton.diamonds/diamonds.json")]
        public void LoadStringSnakeWildOk(string encoded, string decoded)
        {
            var boc = Boc.ParseFromBytes(Convert.FromHexString(encoded));

            var slice = boc.RootCells[0].BeginRead();

            var type = slice.LoadByte();
            Assert.Equal(0x01, type); // off-chain

            var text = slice.LoadStringSnake(true);
            Assert.Equal(decoded, text);
        }

        [Fact]
        public void WritesStringChunkedOk()
        {
            var text = @"Storing a very long text string, expecting it to split to several cell, chained one after another.
One cell can store up to 128 bytes, so lets make this text so it uses at least three cells to store.
This text has length of 283 character now, so this should be enough for our test.";

            var builder = new CellBuilder();

            var cell = builder.StoreStringChunked(text).Build();

            output.WriteLine(cell.ToBoc().DumpCells());

            var slice = cell.BeginRead();
            var text2 = slice.LoadStringChunked();

            Assert.Equal(text, text2);

            slice.EndRead();
        }
    }
}
