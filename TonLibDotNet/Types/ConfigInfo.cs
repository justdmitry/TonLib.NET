namespace TonLibDotNet.Types
{
    [TLSchema("configInfo config:tvm.cell = ConfigInfo")]
    public class ConfigInfo : TypeBase
    {
        public ConfigInfo (Tvm.Cell config)
        {
            this.Config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public Tvm.Cell Config { get; set; }
    }
}
