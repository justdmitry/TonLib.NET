using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    [TLSchema("init options:options = options.Info")]
    public class Init : RequestBase<OptionsInfo>
    {
        public Init(Options options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public Options Options { get; set; }
    }
}
