namespace TonLibDotNet.Types
{
    /// <remarks>
    /// TL Schema:
    /// <code>options.info config_info:options.configInfo = options.Info;</code>
    /// </remarks>
    public class OptionsInfo : TypeBase
    {
        public OptionsInfo()
        {
            TypeName = "options.info";
        }

        public OptionsConfigInfo OptionsConfigInfo { get; set; } = new();
    }
}
