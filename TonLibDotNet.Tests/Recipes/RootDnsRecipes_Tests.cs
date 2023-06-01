namespace TonLibDotNet.Recipes
{
    [Collection(TonClientMainnetCollection.Definition)]
    public class RootDnsRecipes_Tests
    {
        private readonly Func<Task<ITonClient>> getTonClient;

        private const string FoundationTonAddress = "EQB43-VCmf17O7YMd51fAvOjcMkCw46N_3JMCoegH_ZDo40e";

        public RootDnsRecipes_Tests(TonClientMainnetFixture fixture)
        {
            getTonClient = fixture.GetTonClient;
        }

        [Fact]
        public void GetNftIndexWorks()
        {
            var index = TonRecipes.RootDns.GetNftIndex("toncenter.ton");
            var valid = Convert.FromHexString("AFBE954CF1C028D02B3CEDF915D55249F2FCAD54808DB4FB91C7509677373CCF");
            Assert.Equal(valid, index);
        }

        [Fact]
        public async Task ResolveWithSuffix()
        {
            var tonClient = await getTonClient();

            var adr = await TonRecipes.RootDns.GetNftAddress(tonClient, "foundation.ton");
            Assert.Equal(FoundationTonAddress, adr);
        }

        [Fact]
        public async Task ResolveWithoutSuffix()
        {
            var tonClient = await getTonClient();

            var adr = await TonRecipes.RootDns.GetNftAddress(tonClient, "foundation");
            Assert.Equal(FoundationTonAddress, adr);
        }

        [Fact]
        public async Task ResolveNotMinted()
        {
            var tonClient = await getTonClient();

            // Does not exist as NFT at time of writing this test
            var adr = await TonRecipes.RootDns.GetNftAddress(tonClient, "not-yet-minted-domain.ton");

            // How to get expected value?
            // go to https://dns.ton.org/#not-yet-minted-domain and sniff toncenter requests in Developer Console
            Assert.Equal("EQBZoyjQa3ywgrlG_A4lYrhtKxity6BCfMsMEjLNFYSckQGX", adr);
        }

        [Fact]
        public async Task FailsForNonTonDomains()
        {
            var tonClient = await getTonClient();
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => TonRecipes.RootDns.GetNftAddress(tonClient, "alice.t.me"));
        }

        [Theory]
        [InlineData("subdomain.domain.ton")]
        [InlineData("subdomain.domain")]
        public async Task FailsForThirdlevelDomains(string domain)
        {
            var tonClient = await getTonClient();
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => TonRecipes.RootDns.GetNftAddress(tonClient, domain));
        }

        [Fact]
        public async Task GetEditorWorks()
        {
            var tonClient = await getTonClient();
            var ed = await TonRecipes.RootDns.GetEditor(tonClient, FoundationTonAddress);
            Assert.Equal("EQCdqXGvONLwOr3zCNX5FjapflorB6ZsOdcdfLrjsDLt3Fy9", ed);
        }

        [Fact]
        public async Task GetDomainNameWorks()
        {
            var tonClient = await getTonClient();
            var dn = await TonRecipes.RootDns.GetDomainName(tonClient, FoundationTonAddress);
            Assert.Equal("foundation", dn);
        }

        [Fact]
        public async Task GetAllInfoWorks()
        {
            var tonClient = await getTonClient();
            var ti = await TonRecipes.RootDns.GetAllInfo(tonClient, FoundationTonAddress);
            Assert.NotNull(ti);
        }

        [Fact]
        public async Task GetAllInfoByNameWorks()
        {
            var tonClient = await getTonClient();
            var ti = await TonRecipes.RootDns.GetAllInfoByName(tonClient, "foundation.ton");
            Assert.NotNull(ti);
        }
    }
}
