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

            void VerifyDict(Dictionary<byte, ushort> dict)
            {
                Assert.Equal(3, dict.Count);
                Assert.True(dict.ContainsKey(1));
                Assert.True(dict.ContainsKey(17));
                Assert.True(dict.ContainsKey(128));
                Assert.Equal(777, dict[1]);
                Assert.Equal(111, dict[17]);
                Assert.Equal(777, dict[128]);
            }

            var boc = new Boc(cell);

            outputHelper.WriteLine("BOC before parsing:");
            outputHelper.WriteLine(boc.DumpCells());

            var slice = boc.RootCells[0].BeginRead();
            var dict1 = slice.LoadAndParseDict(8, x => x.LoadByte(), x => x.LoadUShort());
            VerifyDict(dict1);

            // Now build it back, and try to parse again
            var cell2 = new CellBuilder()
                .StoreDict(dict1, 8, (x, v) => x.StoreByte(v), (x, v) => x.StoreUShort(v))
                .Build();

            var boc2 = new Boc(cell2);

            outputHelper.WriteLine("Rebuilt BOC, for parsing again:");
            outputHelper.WriteLine(boc2.DumpCells());

            var slice2 = boc2.RootCells[0].BeginRead();
            var dict2 = slice2.LoadAndParseDict(8, x => x.LoadByte(), x => x.LoadUShort());
            VerifyDict(dict2);

            slice2.EndRead();
        }

        [Fact]
        // https://tonscan.org/address/EQB43-VCmf17O7YMd51fAvOjcMkCw46N_3JMCoegH_ZDo40e#source
        // Contract Type: Domain foundation.ton
        public void Sample2IsOk()
        {
            var base64data = $@"
te6cckECCwEAAWcAAtWcfVbMEV589sJeEmvqd8vDyxXVUQby45dWJZEFmWP6o4AW7psr1kCofjDYDWbjVxFa4J78SsJhlfLDEm0
U+hltmfACdqXGvONLwOr3zCNX5FjapflorB6ZsOdcdfLrjsDLt3AAAAAAx1vnNwECAQMAwAMAFGZvdW5kYXRpb24CASAEBQFDv+
TRL8/3fX/2VofmmGKG5JmOf8Wq9nKe97FC4E8L83rrwAYCAW4HCABEdHN/6nryMl8KW2kIk5ydcvkttMDKUsyozixIuycIvxiFQ
QFBvyNRAUIc9uoZap8Fwq0zOZNkIOaNNz9bPcdFOAgVEOxuCQFBvy64EGwLEHtD9ik77A5vHgN1q9KHwxCD0JWGucF3aQ/6CgBJ
n9OAEHv6qlzG5TaOX5eZGIvXmM0i4EqxbR2OpPw3SAdB5jUAEABGrQFRZhjPbL6QBPaIPnQsmi48pT7QLj429M72KpjuHkSRdAD+nQgH";

            var boc = Boc.ParseFromBase64(base64data);

            outputHelper.WriteLine("BOC:");
            outputHelper.WriteLine(boc.DumpCells());

            var slice = boc.RootCells[0].BeginRead();

            slice.SkipBits(256); // index

            outputHelper.WriteLine("Collection address: {0}", slice.LoadAddressIntStd());
            outputHelper.WriteLine("Owner address: {0}", slice.TryLoadAddressIntStd());

            var content = slice.LoadRef();
            var contentSlice = content.BeginRead();
            Assert.Equal(0, contentSlice.LoadByte());
            var dic = contentSlice.LoadAndParseDict(256, x => x.LoadBitsToBytes(256), x => x);

            Assert.Equal(3, dic.Count);
            outputHelper.WriteLine("Content:");
            outputHelper.WriteLine("[0]: {0} = {1}", Convert.ToBase64String(dic.First().Key), "?");
            outputHelper.WriteLine("[1]: {0} = {1}", Convert.ToBase64String(dic.Skip(1).First().Key), "?");
            outputHelper.WriteLine("[2]: {0} = {1}", Convert.ToBase64String(dic.Skip(2).First().Key), "?");

            var domain = slice.LoadRef();
            outputHelper.WriteLine("Domain: {0}.ton", System.Text.Encoding.ASCII.GetString(domain.Content));

            var auction = slice.TryLoadDict();
            if (auction != null)
            {
                var aucSlice = auction.BeginRead();

                outputHelper.WriteLine("Auction last bidder: {0}", aucSlice.LoadAddressIntStd());
                outputHelper.WriteLine("Auction last bid: {0}", aucSlice.LoadCoins());
                outputHelper.WriteLine("Auction end time: {0}", DateTimeOffset.FromUnixTimeSeconds(aucSlice.LoadLong()));

                aucSlice.EndRead();
            }

            outputHelper.WriteLine("Last full-up time: {0}", DateTimeOffset.FromUnixTimeSeconds(slice.LoadLong()));

            slice.EndRead();
        }
    }
}
