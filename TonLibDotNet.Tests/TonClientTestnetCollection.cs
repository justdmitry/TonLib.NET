namespace TonLibDotNet
{
    [CollectionDefinition(Definition)]
    public class TonClientTestnetCollection : ICollectionFixture<TonClientTestnetFixture>
    {
        public const string Definition = "testnet";
    }
}
