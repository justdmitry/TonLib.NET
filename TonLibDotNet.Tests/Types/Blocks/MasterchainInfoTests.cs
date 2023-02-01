namespace TonLibDotNet.Types.Blocks
{
    public class MasterchainInfoTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Blocks_MasterchainInfo);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<MasterchainInfo>(baseObj);

            Assert.NotNull(obj.Last);
            Assert.Equal(-1, obj.Last.Workchain);
            Assert.Equal(-9223372036854775808, obj.Last.Shard);
            Assert.Equal(26984517, obj.Last.Seqno);
            Assert.Equal("k1nEMD+7m+DZAEyktBfy99GTAl/YvFVRBgw3tXr5Xt8=", obj.Last.RootHash);
            Assert.Equal("BKPbMdegxyBMEdeWTNY0RG4SI8Cw7tlqCOQkahlj0cM=", obj.Last.FileHash);

            Assert.Equal("/K7jV7AXp/MKqBL/+3XWpU3kFYp2ObdDG+b9minKUn8=", obj.StateRootHash);

            Assert.NotNull(obj.Init);
            Assert.Equal(-1, obj.Init.Workchain);
            Assert.Equal(0, obj.Init.Shard);
            Assert.Equal(0, obj.Init.Seqno);
            Assert.Equal("F6OpKZKqvqeFp6CQmFomXNMfMj2EnaUSOXN+Mh+wVWk=", obj.Init.RootHash);
            Assert.Equal("XplPz01CXAps5qeSWUtxcyBfdAo5zVb1N979KLSKD24=", obj.Init.FileHash);
        }
    }
}
