namespace TonLibDotNet.Types.Smc
{
    public class InfoTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Smc_Info);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<Info>(baseObj);

            Assert.Equal(1, obj.Id);
        }
    }
}
