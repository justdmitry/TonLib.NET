namespace TonLibDotNet.Types.LiteServer
{
    public class InfoTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.LiteServer_Info);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<Info>(baseObj);

            Assert.Equal(new DateTimeOffset(2023, 2, 1, 10, 19, 33, TimeSpan.Zero), obj.Now);
            Assert.Equal(257, obj.Version);
            Assert.Equal(7, obj.Capabilities);
        }
    }
}
