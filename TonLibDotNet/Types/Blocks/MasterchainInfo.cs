namespace TonLibDotNet.Types.Blocks
{
    /// <remarks>
    /// TL Schema:
    /// <code>blocks.masterchainInfo last:ton.BlockIdExt state_root_hash:bytes init:ton.BlockIdExt = blocks.MasterchainInfo;</code>
    /// </remarks>
    public class MasterchainInfo : TypeBase
    {
        public MasterchainInfo()
        {
            TypeName = "blocks.masterchainInfo";
        }

        public Ton.BlockIdEx Last { get; set; } = new();

        public string StateRootHash { get; set; } = string.Empty;

        public Ton.BlockIdEx Init { get; set; } = new();
    }
}
