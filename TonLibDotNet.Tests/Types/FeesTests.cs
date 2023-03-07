namespace TonLibDotNet.Types
{
    public class FeesTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Fees);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<Fees>(baseObj);

            Assert.Equal(2048000, obj.InFwdFee);
            Assert.Equal(1575, obj.StorageFee);
            Assert.Equal(2994000, obj.GasFee);
            Assert.Equal(1228000, obj.FwdFee);
        }
    }
}
