namespace TonLibDotNet.Cells
{
    public class DictTests
    {
        private readonly Xunit.Abstractions.ITestOutputHelper outputHelper;

        public DictTests(Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this.outputHelper = testOutputHelper;
        }

        [Fact]
        // From https://docs.ton.org/develop/data-formats/tl-b-types#hashmap
        public void SampleIsOk()
        {
            var cell = new CellBuilder() // 1[1]
                .StoreBit(true)
                .StoreRef(new CellBuilder() // 2[00]
                    .StoreBit(false)
                    .StoreBit(false)
                    .StoreRef(new CellBuilder() // 7[1001000]
                        .StoreBits(new[] { true, false, false, true, false, false, false })
                        .StoreRef(new CellBuilder().StoreBits("1010000010000001100001001".Select(x => x == '1').ToArray()))
                        .StoreRef(new CellBuilder().StoreBits("1010000010000000001101111".Select(x => x == '1').ToArray()))
                        )
                    .StoreRef(new CellBuilder().StoreBits("1011100000000000001100001001".Select(x => x == '1').ToArray()))
                    )
                .Build();

            var boc = new Boc(cell);

            outputHelper.WriteLine("BOC before parsing:");
            outputHelper.WriteLine(boc.DumpCells());

            var slice = boc.RootCells[0].BeginRead();
            var dict = slice.LoadAndParseDict(8, x => x.LoadByte(), x => x.LoadUShort());
            slice.EndRead();

            Assert.Equal(3, dict.Count);
            Assert.True(dict.ContainsKey(1));
            Assert.True(dict.ContainsKey(17));
            Assert.True(dict.ContainsKey(128));
            Assert.Equal(777, dict[1]);
            Assert.Equal(111, dict[17]);
            Assert.Equal(777, dict[128]);
        }

        [Fact]
        public void Writing()
        {
            var dict = new Dictionary<byte, ushort>
            {
                [1] = 777,
                [128] = 777,
                [17] = 111
            };

            var cell = new CellBuilder()
                .StoreDict(dict, 8, (x, v) => x.StoreByte(v), (x, v) => x.StoreUShort(v))
                .Build();

            var boc = new Boc(cell);

            outputHelper.WriteLine("Written BOC:");
            outputHelper.WriteLine(boc.DumpCells());

            var base64 = boc.SerializeToBase64();

            var boc2 = Boc.ParseFromBase64(base64);
            var slice2 = boc2.RootCells[0].BeginRead();
            var dict2 = slice2.LoadAndParseDict(8, x => x.LoadByte(), x => x.LoadUShort());
            slice2.EndRead();

            Assert.Equal(3, dict2.Count);
            Assert.Equal(777, dict2[1]);
            Assert.Equal(111, dict2[17]);
            Assert.Equal(777, dict2[128]);
        }

        [Fact]
        public void Writing2()
        {
            var dict = new Dictionary<byte, ushort>
            {
                [0] = 10,
                [1] = 9,
                [2] = 8,
                [3] = 7,
            };

            var cell = new CellBuilder()
                .StoreDict(dict, 32, (x, v) => x.StoreInt(v), (x, v) => x.StoreInt(v))
                .Build();

            var boc = new Boc(cell);

            outputHelper.WriteLine("Written BOC:");
            outputHelper.WriteLine(boc.DumpCells());

            var base64 = boc.SerializeToBase64();

            var boc2 = Boc.ParseFromBase64(base64);
            var slice2 = boc2.RootCells[0].BeginRead();
            var dict2 = slice2.LoadAndParseDict(32, x => x.LoadInt(), x => x.LoadInt());
            slice2.EndRead();

            Assert.Equal(4, dict2.Count);
            Assert.Equal(10, dict2[0]);
            Assert.Equal(9, dict2[1]);
            Assert.Equal(8, dict2[2]);
            Assert.Equal(7, dict2[3]);
        }
    }
}
