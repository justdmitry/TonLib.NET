using System.Diagnostics.CodeAnalysis;
using TonLibDotNet.Requests;
using TonLibDotNet.Types;

namespace TonLibDotNet
{
    public interface ITonClient
    {
        OptionsInfo? OptionsInfo { get; }

        Task<OptionsInfo?> InitIfNeeded();

        Task<OptionsInfo?> Reinit();

        Task<TResponse> Execute<TResponse>(RequestBase<TResponse> request)
            where TResponse : TypeBase;

        [Obsolete("Use TonUtils.Coins.FromNano()")]
        decimal ConvertFromNanoTon(long nano);

        [Obsolete("Use TonUtils.Coins.ToNano()")]
        long ConvertToNanoTon(decimal ton);

        [Obsolete("Use TonUtils.Text.EncodeAsBase64")]
        [return: NotNullIfNotNull(nameof(source))]
        string? EncodeStringAsBase64(string? source);

        [Obsolete("Use TonUtils.Text.TryDecodeBase64")]
        bool TryDecodeBase64AsString(string? source, [NotNullWhen(true)] out string? result);
    }
}
