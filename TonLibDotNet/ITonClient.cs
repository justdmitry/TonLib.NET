using System.Diagnostics.CodeAnalysis;
using TonLibDotNet.Requests;
using TonLibDotNet.Types;

namespace TonLibDotNet
{
    public interface ITonClient
    {
        OptionsInfo? OptionsInfo { get; }

		/// <summary>
		/// Current masterchain height (TonClient synced to LiteServer synced to blockchain).
		/// </summary>
		int SyncStateCurrentSeqno { get; }

        /// <summary>
        /// Initialize TonClient: choose a LiteServer and try to connect to it.
        /// </summary>
        /// <remarks>
        /// Will do nothing, if already connected.
        /// </remarks>
        /// <returns>Information about blockchain params, e.g. <see cref="OptionsConfigInfo.DefaultWalletId">DefaultWalletId</see>.</returns>
        Task<OptionsInfo?> InitIfNeeded();

        /// <summary>
        /// De-initialize TonClient. Next call to <see cref="InitIfNeeded"/> will choose LiteServer again and connect to it.
        /// </summary>
        /// <remarks>Useful when you find currently connected LiteServer is not synced, for example.</remarks>
        void Deinit();

        /// <summary>
        /// De-initialize and immediately initialize again.
        /// </summary>
        /// <remarks>
        /// Equivalent of:
        /// <code>
        ///   Deinit();
        ///   InitIfNeeded();
        /// </code>
        /// </remarks>
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
