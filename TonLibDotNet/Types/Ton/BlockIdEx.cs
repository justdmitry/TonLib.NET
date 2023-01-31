namespace TonLibDotNet.Types.Ton
{
    /// <remarks>
    /// TL Schema:
    /// <code>ton.blockIdExt workchain:int32 shard:int64 seqno:int32 root_hash:bytes file_hash:bytes = ton.BlockIdExt;</code>
    /// </remarks>
    public class BlockIdEx : TypeBase
    {
        public BlockIdEx()
        {
            TypeName = "ton.blockIdExt";
        }

        public int Workchain { get; set; }

        public long Shard { get; set; }

        public int Seqno { get; set; }

        public string RootHash { get; set; } = string.Empty;

        public string FileHash { get; set; } = string.Empty;
    }
}
