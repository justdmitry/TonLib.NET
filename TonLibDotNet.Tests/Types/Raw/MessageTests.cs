namespace TonLibDotNet.Types.Raw
{
    public class MessageTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Raw_Message);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<Message>(baseObj);

            Assert.NotNull(obj.Source);
            Assert.Equal("EQAyAAlbgteBHCuI50EzuVvu9Zrk4Ts7pKU3bsphGBcyY0O_", obj.Source.Value);

            Assert.NotNull(obj.Destination);
            Assert.Equal("EQCJTkhd1W2wztkVNp_dsKBpv2SIoUWoIyzI7mQrbSrj_NSh", obj.Destination.Value);

            Assert.Equal(880000000, obj.Value);
            Assert.Equal(666672, obj.FwdFee);
            Assert.Equal(1, obj.IhrFee);
            Assert.Equal(34948872000005, obj.CreatedLt);
            Assert.Equal("lqKW0iTyhcZ77pPDD4owkVfw2qNdxbh+QQt4YwoJz8c=", obj.BodyHash);

            Assert.NotNull(obj.MsgData);
            var md = Assert.IsType<Msg.DataRaw>(obj.MsgData);
            Assert.Equal("te6cckEBAQEAAgAAAEysuc0=", md.Body);
            Assert.Equal("12345", md.InitState);
        }
    }
}
