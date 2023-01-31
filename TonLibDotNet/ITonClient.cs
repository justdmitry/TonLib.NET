using TonLibDotNet.Requests;
using TonLibDotNet.Types;

namespace TonLibDotNet
{
    public interface ITonClient
    {
        Task<OptionsInfo?> InitIfNeeded();

        TResponse Execute<TResponse>(RequestBase<TResponse> request)
            where TResponse : TypeBase;
    }
}
