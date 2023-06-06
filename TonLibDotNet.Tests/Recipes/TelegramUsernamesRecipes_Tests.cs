namespace TonLibDotNet.Recipes
{
    [Collection(TonClientMainnetCollection.Definition)]
    public class TelegramUsernamesRecipes_Tests
    {
        private readonly Func<Task<ITonClient>> getTonClient;

        private readonly TelegramUsernamesRecipes instance = TelegramUsernamesRecipes.Instance;

        private const string ValidNameFull = "welcome.t.me";
        private const string ValidNameShort = "welcome";
        private const string ValidNameAddress = "EQAngajSUhC0DfUN1OHp33-6w06Mt33fjm4KcwzMU3bGXx_E";

        public TelegramUsernamesRecipes_Tests(TonClientMainnetFixture fixture)
        {
            getTonClient = fixture.GetTonClient;
        }

        [Theory]
        [InlineData("wElCoMe.t.me", "welcome")]
        [InlineData("wELCOME.T.ME.", "welcome")]
        [InlineData("welcome", "welcome")]
        [InlineData("welcome.", "welcome")]
        [InlineData("not-yet-minted-name.t.me", "not-yet-minted-name")]
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
        [InlineData("subdomain.domain.t.me")]
        [InlineData("subdomain.domain")]
        [InlineData("welcome.ton")]
        public void NormalizeNameRejectsInvalid(string name)
        {
            Assert.False(instance.TryNormalizeName(name, out _));
        }

        [Theory]
        [InlineData(ValidNameFull)]
        [InlineData(ValidNameShort)]
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
            var adr = await instance.GetNftAddress(tonClient, "not-yet-minted-name.t.me");

            // How to get expected value?
            // Replace with Assert.Equal if you know how.
            Assert.False(string.IsNullOrEmpty(adr));
        }

        [Fact]
        public async Task GetFullDomainWorks()
        {
            var tonClient = await getTonClient();
            var dn = await instance.GetFullDomain(tonClient, ValidNameAddress);
            Assert.Equal(ValidNameFull, dn);
        }

        [Fact]
        public async Task GetTokenNameWorks()
        {
            var tonClient = await getTonClient();
            var dn = await instance.GetTokenName(tonClient, ValidNameAddress);
            Assert.Equal(ValidNameShort, dn);
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
            var ti = await instance.GetAllInfoByName(tonClient, ValidNameFull);
            Assert.NotNull(ti);
        }
    }
}
