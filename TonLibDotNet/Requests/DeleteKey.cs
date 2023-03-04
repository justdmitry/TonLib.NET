using TonLibDotNet.Types;

namespace TonLibDotNet.Requests
{
    [TLSchema("deleteKey key:key = Ok")]
    public class DeleteKey : RequestBase<Ok>
    {
        public DeleteKey(Key key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public Key Key { get; set; }
    }
}
