namespace TonLibDotNet.Types.Msg
{
    public class MessageTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Msg_Message);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<Message>(baseObj);

            Assert.NotNull(obj.Destination);
            Assert.Equal("EQAk...mdT0", obj.Destination.Value);

            Assert.Equal("PuZi...pU9", obj.PublicKey);
            Assert.Equal(10000000, obj.Amount);
            Assert.Equal(1, obj.SendMode);

            var dat = Assert.IsType<DataText>(obj.Data);
            Assert.Equal("U2VudCB1c2luZyBodHRwczovL2dpdGh1Yi5jb20vanVzdGRtaXRyeS9Ub25MaWIuTkVU", dat.Text);
        }
    }
}
