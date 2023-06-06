namespace TonLibDotNet.Recipes
{
    [Collection(TonClientMainnetCollection.Definition)]
    public class TelegramNumbersRecipes_Tests
    {
        private readonly Func<Task<ITonClient>> getTonClient;

        private readonly TelegramNumbersRecipes instance = TelegramNumbersRecipes.Instance;

        private const string ValidName = "88800008888";
        private const string ValidNameAddress = "EQByytVzsVlfuuIALs0b6X_0SrXHXNqBRXPf4S1P6Nm6ZNF4";

        public TelegramNumbersRecipes_Tests(TonClientMainnetFixture fixture)
        {
            getTonClient = fixture.GetTonClient;
        }

        [Theory]
        [InlineData("88800008888", "88800008888")]
        [InlineData("+888 0000 8888", "88800008888")]
        [InlineData("+888-0000-8888", "88800008888")]
        [InlineData("+888 (0000) 8888", "88800008888")]
        [InlineData("888 8888 8888 8888", "888888888888888")]
        public void NormalizeNameWorks(string name, string expected)
        {
            Assert.True(instance.TryNormalizeName(name, out var normalizedName));
            Assert.Equal(expected, normalizedName);
        }

        [Fact]
        public void NormalizeNameRejectsEmpty()
        {
            Assert.Throws<ArgumentNullException>(() => instance.TryNormalizeName(null, out _));
            Assert.Throws<ArgumentNullException>(() => instance.TryNormalizeName(string.Empty, out _));
        }

        [Theory]
        [InlineData("888~00008888")]
        [InlineData("888a00008888")]
        [InlineData("77700008888")]
        public void NormalizeNameRejectsInvalid(string name)
        {
            Assert.False(instance.TryNormalizeName(name, out _));
        }

        [Theory]
        [InlineData(ValidName)]
        public async Task ResolveMinted(string name)
        {
            var tonClient = await getTonClient();

            var adr = await instance.GetNftAddress(tonClient, name);
            Assert.Equal(ValidNameAddress, adr);
        }

        [Fact]
        public async Task ResolveNotMinted()
        {
            var tonClient = await getTonClient();

            // Does not exist as NFT at time of writing this test
            var adr = await instance.GetNftAddress(tonClient, "888 8888 8888 8888");

            // How to get expected value?
            // Replace with Assert.Equal if you know how.
            Assert.False(string.IsNullOrEmpty(adr));
        }

        [Fact]
        public async Task GetTokenNameWorks()
        {
            var tonClient = await getTonClient();
            var dn = await instance.GetTokenName(tonClient, ValidNameAddress);
            Assert.Equal(ValidName, dn);
        }

        [Fact]
        public async Task GetAllInfoWorks()
        {
            var tonClient = await getTonClient();
            var ti = await instance.GetAllInfo(tonClient, ValidNameAddress);
            Assert.NotNull(ti);
        }

        [Fact]
        public async Task GetAllInfoByNameWorks()
        {
            var tonClient = await getTonClient();
            var ti = await instance.GetAllInfoByName(tonClient, ValidName);
            Assert.NotNull(ti);
        }
    }
}
