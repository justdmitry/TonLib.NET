using Xunit.Abstractions;

namespace TonLibDotNet.Cells
{
    public class DictTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        public DictTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
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
            _testOutputHelper.WriteLine("BOC before parsing:");
            _testOutputHelper.WriteLine(boc.DumpCells());

            var slice = boc.RootCells[0].BeginRead();
            var dict = slice.LoadDict(8, x => x.LoadByte(), x => x.LoadUShort());

            Assert.Equal(3, dict.Count);
            Assert.True(dict.ContainsKey(1));
            Assert.True(dict.ContainsKey(17));
            Assert.True(dict.ContainsKey(128));
            Assert.Equal(777, dict[1]);
            Assert.Equal(111, dict[17]);
            Assert.Equal(777, dict[128]);

            // Now build it back, and try to parse again
            var cell2 = new CellBuilder()
                .StoreDict(dict, (x, v) => x.StoreByte(v), (x, v) => x.StoreUShort(v))
                .Build();
            var boc2 = new Boc(cell2);
            _testOutputHelper.WriteLine("Rebuilt BOC, for parsing again:");
            _testOutputHelper.WriteLine(boc2.DumpCells());

            var slice2 = boc2.RootCells[0].BeginRead();
            var dict2 = slice2.LoadDict(8, x => x.LoadByte(), x => x.LoadUShort());

            Assert.Equal(3, dict2.Count);
            Assert.True(dict2.ContainsKey(1));
            Assert.True(dict2.ContainsKey(17));
            Assert.True(dict2.ContainsKey(128));
            Assert.Equal(777, dict2[1]);
            Assert.Equal(111, dict2[17]);
            Assert.Equal(777, dict2[128]);
        }
    }
}
