namespace TonLibDotNet.Types.Uninited
{
    public class AccountStateTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Uninited_AccountState);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<AccountState>(baseObj);

            Assert.Equal("---", obj.FrozenHash);
        }
    }
}
