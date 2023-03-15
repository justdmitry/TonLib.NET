using System.Text.Json.Serialization;

namespace TonLibDotNet.Types
{
    [TLSchema("adnlAddress adnl_address:string = AdnlAddress")]
    public class AdnlAddress : TypeBase
    {
        public AdnlAddress(string value)
        {
            Value = value;
        }

        [JsonPropertyName("adnl_address")]
        public string Value { get; set; }
    }
}
