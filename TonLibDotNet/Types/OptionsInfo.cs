namespace TonLibDotNet.Types
{
    [TLSchema("options.info config_info:options.configInfo = options.Info")]
    public class OptionsInfo : TypeBase
    {
        public OptionsConfigInfo ConfigInfo { get; set; } = new();
    }
}
