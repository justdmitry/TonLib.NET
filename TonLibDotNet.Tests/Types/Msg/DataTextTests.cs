namespace TonLibDotNet.Types.Msg
{
    public class DataTextTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Msg_DataText);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<DataText>(baseObj);

            Assert.Equal("Um95YWx0eQ==", obj.Text);
        }
    }
}
