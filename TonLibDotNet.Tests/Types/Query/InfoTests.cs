namespace TonLibDotNet.Types.Query
{
    public class InfoTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Query_Info);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<Info>(baseObj);

            Assert.Equal(1, obj.Id);
            Assert.Equal(1678173619, obj.ValidUntil);
            Assert.Equal("XWY...NdE=", obj.BodyHash);
            Assert.Equal("te6...BL1U=", obj.Body);
            Assert.Equal("???", obj.InitState);
        }
    }
}
