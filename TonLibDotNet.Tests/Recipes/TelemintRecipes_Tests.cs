namespace TonLibDotNet.Recipes
{
    [Collection(TonClientMainnetCollection.Definition)]
    public class TelemintRecipes_Tests
    {
        private readonly Func<Task<ITonClient>> getTonClient;

        private const string WelcomeTMeAddress = "EQAngajSUhC0DfUN1OHp33-6w06Mt33fjm4KcwzMU3bGXx_E";

        public TelemintRecipes_Tests(TonClientMainnetFixture fixture)
        {
            getTonClient = fixture.GetTonClient;
        }

        [Fact]
        public async Task ResolveWithSuffix()
        {
            var tonClient = await getTonClient();

            var adr = await TonRecipes.Telemint.GetNftAddress(tonClient, "welcome.t.me");
            Assert.Equal(WelcomeTMeAddress, adr);
        }

        [Fact]
        public async Task ResolveWithoutSuffix()
        {
            var tonClient = await getTonClient();

            var adr = await TonRecipes.Telemint.GetNftAddress(tonClient, "welcome");
            Assert.Equal(WelcomeTMeAddress, adr);
        }

        [Fact]
        public async Task ResolveNotMinted()
        {
            var tonClient = await getTonClient();

            // Does not exist as NFT at time of writing this test
            var adr = await TonRecipes.Telemint.GetNftAddress(tonClient, "not-yet-minted-name.t.me");

            // How to get expected value?
            // Replace with Assert.Equal if you know how.
            Assert.False(string.IsNullOrEmpty(adr));
        }

        [Fact]
        public async Task FailsForOtherDomains()
        {
            var tonClient = await getTonClient();
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => TonRecipes.Telemint.GetNftAddress(tonClient, "alice.ton"));
        }

        [Theory]
        [InlineData("subdomain.domain.t.me")]
        [InlineData("subdomain.domain")]
        public async Task FailsForThirdlevelDomains(string domain)
        {
            var tonClient = await getTonClient();
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => TonRecipes.Telemint.GetNftAddress(tonClient, domain));
        }

        [Fact]
        public async Task GetFullDomainWorks()
        {
            var tonClient = await getTonClient();
            var dn = await TonRecipes.Telemint.GetFullDomain(tonClient, WelcomeTMeAddress);
            Assert.Equal("welcome.t.me", dn);
        }

        [Fact]
        public async Task GetTokenNameWorks()
        {
            var tonClient = await getTonClient();
            var dn = await TonRecipes.Telemint.GetTokenName(tonClient, WelcomeTMeAddress);
            Assert.Equal("welcome", dn);
        }

        [Fact]
        public async Task GetAllInfoWorks()
        {
            var tonClient = await getTonClient();
            var ti = await TonRecipes.Telemint.GetAllInfo(tonClient, WelcomeTMeAddress);
            Assert.NotNull(ti);
        }

        [Fact]
        public async Task GetAllInfoByNameWorks()
        {
            var tonClient = await getTonClient();
            var ti = await TonRecipes.Telemint.GetAllInfoByName(tonClient, "welcome.t.me");
            Assert.NotNull(ti);
        }
    }
}
