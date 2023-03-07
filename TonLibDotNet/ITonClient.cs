using System.Diagnostics.CodeAnalysis;
using TonLibDotNet.Requests;
using TonLibDotNet.Types;

namespace TonLibDotNet
{
    public interface ITonClient
    {
        OptionsInfo OptionsInfo { get; }

        Task<OptionsInfo?> InitIfNeeded();

        Task<OptionsInfo?> Reinit();

        Task<TResponse> Execute<TResponse>(RequestBase<TResponse> request)
            where TResponse : TypeBase;

        decimal ConvertFromNanoTon(long nano);

        long ConvertToNanoTon(decimal ton);

        [return: NotNullIfNotNull("source")]
        string? EncodeStringAsBase64(string? source);

        bool TryDecodeBase64AsString(string? source, [NotNullWhen(true)] out string? result);
    }
}
