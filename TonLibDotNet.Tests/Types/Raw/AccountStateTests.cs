namespace TonLibDotNet.Types.Raw
{
    public class AccountStateTests
    {
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Raw_AccountState);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<AccountState>(baseObj);

            Assert.Equal("te6...zJw==", obj.Code);
            Assert.Equal("te6...CWaB", obj.Data);
            Assert.Equal("???", obj.FrozenHash);
        }
    }
}
