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
                        .StoreRef(new CellBuilder() // 25[1010 0000 1000 0001 1000 01001]
                            .StoreBits(new[] {
                                true, false, true, false,
                                false, false, false, false,
                                true, false, false, false,
                                false, false, false, true,
                                true, false, false, false,
                                false, true, false, false, true})
                            )
                        .StoreRef(new CellBuilder() // 25[1010 0000 1000 0000 0011 01111]
                            .StoreBits(new[] {
                                true, false, true, false,
                                false, false, false, false,
                                true, false, false, false,
                                false, false, false, false,
                                false, false, true, true,
                                false, true, true, true, true})
                            )
                        )
                    .StoreRef(new CellBuilder() // 28[1011 1000 0000 0000 0011 0000 1001]
                        .StoreBits(new[] {
                            true, false, true, true,
                            true, false, false, false,
                            false, false, false, false,
                            false, false, false, false,
                            false, false, true, true,
                            false, false, false, false,
                            true, false, false, true})
                        )
                    )
                .Build();
            var boc = new Boc(cell);
            _testOutputHelper.WriteLine(boc.DumpCells());

            var slice = boc.RootCells[0].BeginRead();
            var dict = slice.LoadDict(8, 16, x => x.LoadByte(), x => x.LoadUShort());

            Assert.Equal(3, dict.Count);
            Assert.True(dict.ContainsKey(1));
            Assert.True(dict.ContainsKey(17));
            Assert.True(dict.ContainsKey(128));
            Assert.Equal(777, dict[1]);
            Assert.Equal(111, dict[17]);
            Assert.Equal(777, dict[128]);
        }
    }
}
