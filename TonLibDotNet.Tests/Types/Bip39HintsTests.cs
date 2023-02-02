namespace TonLibDotNet.Types
{
    public class Bip39HintsTests
    {
        [Fact]
        public void DeserializeOk()
        {
            var baseObj = new TonJsonSerializer().Deserialize(SampleValues.Bip39Hints);

            Assert.NotNull(baseObj);

            var obj = Assert.IsType<Bip39Hints>(baseObj);

            Assert.NotEmpty(obj.Words);
            Assert.Equal(new[] { "auction", "audit", "august", "aunt", "author", "auto", "autumn" }, obj.Words);
        }

    }
}
