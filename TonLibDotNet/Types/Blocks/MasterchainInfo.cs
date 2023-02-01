namespace TonLibDotNet.Types.Blocks
{
    [TLSchema("blocks.masterchainInfo last:ton.BlockIdExt state_root_hash:bytes init:ton.BlockIdExt = blocks.MasterchainInfo")]
    public class MasterchainInfo : TypeBase
    {
        public Ton.BlockIdEx Last { get; set; } = new();

        public string StateRootHash { get; set; } = string.Empty;

        public Ton.BlockIdEx Init { get; set; } = new();
    }
}
