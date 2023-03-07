namespace TonLibDotNet.Types.Query
{
    public class FeesTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Query_Fees);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<Fees>(baseObj);

            Assert.NotNull(obj.SourceFees);
            Assert.Equal(2048000, obj.SourceFees.InFwdFee);
            Assert.Equal(1575, obj.SourceFees.StorageFee);
            Assert.Equal(2994000, obj.SourceFees.GasFee);
            Assert.Equal(1228000, obj.SourceFees.FwdFee);

            Assert.NotNull(obj.DestinationFees);
            Assert.Single(obj.DestinationFees);
            Assert.NotNull(obj.DestinationFees[0]);

            Assert.Equal(0, obj.DestinationFees[0].InFwdFee);
            Assert.Equal(1575, obj.DestinationFees[0].StorageFee);
            Assert.Equal(100000, obj.DestinationFees[0].GasFee);
            Assert.Equal(0, obj.DestinationFees[0].FwdFee);
        }
    }
}
