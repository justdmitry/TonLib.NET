namespace TonLibDotNet.Types.Query
{
    [TLSchema("query.fees source_fees:fees destination_fees:vector<fees> = query.Fees")]
    public class Fees : TypeBase
    {
        public Fees(Types.Fees sourceFees)
        {
            SourceFees = sourceFees ?? throw new ArgumentNullException(nameof(sourceFees));
        }

        public Types.Fees SourceFees { get; set; }

        public List<Types.Fees> DestinationFees { get; set; } = new();
    }
}
