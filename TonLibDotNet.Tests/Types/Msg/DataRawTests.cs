namespace TonLibDotNet.Types.Msg
{
    public class DataRawTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Msg_DataRaw);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<DataRaw>(baseObj);

            Assert.Equal("te6cckEBAQEAAgAAAEysuc0=", obj.Body);
            Assert.Equal("???", obj.InitState);
        }
    }
}
