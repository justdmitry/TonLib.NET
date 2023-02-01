using TonLibDotNet.Types;

namespace TonLibDotNet.Utils
{
    public interface ITonJsonSerializer
    {
        string Serialize(TypeBase type);

        TypeBase? Deserialize(string json);
    }
}
