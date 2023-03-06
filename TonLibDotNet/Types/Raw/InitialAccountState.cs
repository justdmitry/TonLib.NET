namespace TonLibDotNet.Types.Raw
{
    [TLSchema("raw.initialAccountState code:bytes data:bytes = InitialAccountState")]
    public class InitialAccountState : Types.InitialAccountState
    {
        public string Code { get; set; } = string.Empty;

        public string Data { get; set; } = string.Empty;
    }
}
