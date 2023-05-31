namespace TonLibDotNet.Recipes
{
    [Collection(TonClientMainnetCollection.Definition)]
    public class RootDnsRecipes_GetNftAddress_Tests
    {
        private readonly Func<Task<ITonClient>> getTonClient;

        public RootDnsRecipes_GetNftAddress_Tests(TonClientMainnetFixture fixture)
        {
            getTonClient = fixture.GetTonClient;
        }

        [Fact]
        public async Task ResolveWithTonSuffix()
        {
            var tonClient = await getTonClient();

            var adr = await TonRecipes.RootDns.GetNftAddress(tonClient, "foundation.ton");
            Assert.Equal("EQB43-VCmf17O7YMd51fAvOjcMkCw46N_3JMCoegH_ZDo40e", adr);
        }

        [Fact]
        public async Task ResolveWithoutTonSuffix()
        {
            var tonClient = await getTonClient();

            var adr = await TonRecipes.RootDns.GetNftAddress(tonClient, "foundation");
            Assert.Equal("EQB43-VCmf17O7YMd51fAvOjcMkCw46N_3JMCoegH_ZDo40e", adr);
        }

        [Fact]
        public async Task ResolveNotMinted()
        {
            var tonClient = await getTonClient();

            // Does not exist as NFT at time of writing this test
            var adr = await TonRecipes.RootDns.GetNftAddress(tonClient, "not-yet-minted-domain.ton");

            // how to get expected value?
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
    }
}
