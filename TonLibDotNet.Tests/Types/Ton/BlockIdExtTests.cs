namespace TonLibDotNet.Types.Ton
{
    public class BlockIdExtTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Ton_BlockIdEx);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<BlockIdEx>(baseObj);

            Assert.Equal(-1, obj.Workchain);
            Assert.Equal(-9223372036854775808, obj.Shard);
            Assert.Equal(26984517, obj.Seqno);
            Assert.Equal("k1nEMD+7m+DZAEyktBfy99GTAl/YvFVRBgw3tXr5Xt8=", obj.RootHash);
            Assert.Equal("BKPbMdegxyBMEdeWTNY0RG4SI8Cw7tlqCOQkahlj0cM=", obj.FileHash);
        }
    }
}
