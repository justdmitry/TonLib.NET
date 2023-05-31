namespace TonLibDotNet
{
    [CollectionDefinition(Definition)]
    public class TonClientMainnetCollection : ICollectionFixture<TonClientMainnetFixture>
    {
        public const string Definition = "mainnet";
    }
}
