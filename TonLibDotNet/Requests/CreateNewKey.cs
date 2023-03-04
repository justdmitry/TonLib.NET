using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    [TLSchema("createNewKey local_password:secureBytes mnemonic_password:secureBytes random_extra_seed:secureBytes = Key")]
    public class CreateNewKey : RequestBase<Key>
    {
        public string? LocalPassword { get; set; }

        public string? MnemonicPassword { get; set; }

        public string? RandomExtraSeed { get; set; }
    }
}
