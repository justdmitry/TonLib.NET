using System.Diagnostics.CodeAnalysis;
using TonLibDotNet.Requests;
using TonLibDotNet.Types;

namespace TonLibDotNet
{
    public interface ITonClient
    {
        Task<OptionsInfo?> InitIfNeeded();

        Task<TResponse> Execute<TResponse>(RequestBase<TResponse> request)
            where TResponse : TypeBase;

        decimal ConvertFromNanoTon(long nano);

        long ConvertToNanoTon(decimal ton);

        bool TryDecodeBase64AsString(string? source, [NotNullWhen(true)] out string? result);
    }
}
