namespace TonLibDotNet.Types.Ton
{
    [TLSchema("ton.blockIdExt workchain:int32 shard:int64 seqno:int32 root_hash:bytes file_hash:bytes = ton.BlockIdExt")]
    public class BlockIdEx : TypeBase
    {
        public int Workchain { get; set; }

        public long Shard { get; set; }

        public int Seqno { get; set; }

        public string RootHash { get; set; } = string.Empty;

        public string FileHash { get; set; } = string.Empty;
    }
}
