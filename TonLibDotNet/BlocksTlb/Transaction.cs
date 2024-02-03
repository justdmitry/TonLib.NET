using TonLibDotNet.Cells;

namespace TonLibDotNet.BlocksTlb
{
    /// <summary>
    /// Transaction information.
    /// </summary>
    /// <remarks>
    /// <code>
    /// transaction$0111 account_addr:bits256 lt:uint64
    ///   prev_trans_hash:bits256 prev_trans_lt:uint64 now:uint32
    ///   outmsg_cnt:uint15
    ///   orig_status:AccountStatus end_status:AccountStatus
    ///   ^[in_msg:(Maybe ^(Message Any)) out_msgs:(HashmapE 15 ^(Message Any)) ]
    ///   total_fees:CurrencyCollection state_update:^(HASH_UPDATE Account)
    ///   description:^TransactionDescr = Transaction;
    /// </code>
    /// </remarks>
    public class Transaction
    {
        public Transaction(Slice src)
        {
            var prefix = src.PreloadUInt(4);

            if (prefix != 0b0111)
            {
                throw new InvalidOperationException("Invalid prefix: 0111 expected");
            }

            src.SkipBits(4); // preloaded prefix

            AccountAddr = src.LoadBitsToBytes(256);
            Lt = src.LoadULong(64);
            PrevTransHash = src.LoadBitsToBytes(256);
            PrevTransLt = src.LoadULong(64);
            Now = src.LoadUInt(32);
            OutmsgCnt = src.LoadUInt(15);

            OriginalStatus = AccountStatus.CreateFrom(src);
            EndStatus = AccountStatus.CreateFrom(src);

            src.SkipRef(); // in/out messages

            TotalFees = new CurrencyCollection(src);

            src.SkipRef(); // state_update

            Description = TransactionDescr.CreateFrom(src.LoadRef().BeginRead());
        }

        public byte[] AccountAddr { get; set; } = Array.Empty<byte>();

        public ulong Lt { get; set; }

        public byte[] PrevTransHash { get; set; } = Array.Empty<byte>();

        public ulong PrevTransLt { get; set; }

        public uint Now { get; set; }

        public uint OutmsgCnt { get; set;}

        public AccountStatus OriginalStatus { get; set; }

        public AccountStatus EndStatus { get; set; }

        public CurrencyCollection TotalFees { get; set; }

        public TransactionDescr? Description { get; set; }
    }
}
