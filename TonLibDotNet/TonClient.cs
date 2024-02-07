using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using TonLibDotNet.Requests;
using TonLibDotNet.Types;

namespace TonLibDotNet
{
    public class TonClient : ITonClient, IDisposable
    {
        private readonly ILogger logger;
        private readonly TonOptions tonOptions;

        private readonly SemaphoreSlim syncRoot = new (1);

        private IntPtr? client;
        private bool initialized;
        private bool needReinit;
        private bool isDisposed;

        static TonClient()
        {
            TonLibResolver.Register(typeof(TonClient).Assembly);
        }

        public TonClient(ILogger<TonClient> logger, Microsoft.Extensions.Options.IOptions<TonOptions> options)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.tonOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        [DllImport(TonLibResolver.DllNamePlaceholder)]
        private static extern IntPtr tonlib_client_json_create();

        [DllImport(TonLibResolver.DllNamePlaceholder)]
        private static extern void tonlib_client_json_destroy(IntPtr client);

        [DllImport(TonLibResolver.DllNamePlaceholder)]
        private static extern void tonlib_client_set_verbosity_level(int level);

        [DllImport(TonLibResolver.DllNamePlaceholder, CharSet = CharSet.Ansi)]
        private static extern IntPtr tonlib_client_json_execute(IntPtr client, string request);

        [DllImport(TonLibResolver.DllNamePlaceholder, CharSet = CharSet.Ansi)]
        private static extern void tonlib_client_json_send(IntPtr client, string request);

        [DllImport(TonLibResolver.DllNamePlaceholder)]
        private static extern IntPtr tonlib_client_json_receive(IntPtr client, double timeout);

        public OptionsInfo? OptionsInfo { get; private set; }

        /// <inheritdoc />
        public int SyncStateCurrentSeqno { get; private set; }

        /// <summary>
        /// Add assembly with additional <see cref="TypeBase"/> classes for LiteServer interaction.
        /// </summary>
        /// <param name="assembly">Assembly to add</param>
        public static void RegisterAssembly(System.Reflection.Assembly assembly)
        {
            TonTypeResolver.AdditionalAsseblies.Add(assembly);
        }

        /// <inheritdoc />
        public async Task<OptionsInfo?> InitIfNeeded()
        {
            if (needReinit)
            {
                logger.LogDebug("Reinitializing...");

                if (!await syncRoot.WaitAsync(tonOptions.ConcurrencyTimeout))
                {
                    throw new TimeoutException("Failed while waiting for semaphore");
                }

                if (client != null)
                {
                    tonlib_client_json_destroy(client.Value);
                    client = null;
                }

                initialized = false;
                needReinit = false;

                syncRoot.Release();
            }

            if (initialized)
            {
                return null;
            }

            string fullConfig;
            var localConfigSource = tonOptions.UseMainnet ? tonOptions.ConfigPathLocalMainnet : tonOptions.ConfigPathLocalTestnet;
            if (!string.IsNullOrEmpty(localConfigSource))
            {
                fullConfig = await File.ReadAllTextAsync(localConfigSource);
                logger.LogDebug("Used local config file: {Name}", localConfigSource);
            }
            else
            {
                var remoteConfigSource = tonOptions.UseMainnet ? tonOptions.ConfigPathMainnet : tonOptions.ConfigPathTestnet;
                using var httpClient = new HttpClient();
                fullConfig = await httpClient.GetStringAsync(remoteConfigSource).ConfigureAwait(false);
                logger.LogDebug("Used internet config file: {Url}", remoteConfigSource);
            }

            var jdoc = JsonNode.Parse(fullConfig);
            var servers = jdoc["liteservers"].AsArray();
            var choosen = tonOptions.LiteServerSelector(servers);
            servers.Clear();
            servers.Add(choosen);
            logger.LogInformation("LiteServer choosen: ip={IP}, port={Port}, key={Key}", choosen["ip"], choosen["port"], choosen["id"]?["key"]);

            tonOptions.Options.Config.ConfigJson = jdoc.ToJsonString();

            OptionsInfo = await Execute(new Init(tonOptions.Options));
            return OptionsInfo;
        }

        /// <inheritdoc />
        public virtual void Deinit()
        {
            logger.LogWarning("De-initializing.");
            needReinit = true;
        }

        /// <inheritdoc />
        public Task<OptionsInfo?> Reinit()
        {
            Deinit();
            return InitIfNeeded();
        }

        /// <inheritdoc />
        public async Task<TResponse> Execute<TResponse>(RequestBase<TResponse> request)
            where TResponse : TypeBase
        {
            if (client == null)
            {
                if (!await syncRoot.WaitAsync(tonOptions.ConcurrencyTimeout))
                {
                    throw new TimeoutException("Failed while waiting for semaphore");
                }

                try
                {
                    if (client == null)
                    {
                        tonlib_client_set_verbosity_level(tonOptions.VerbosityLevel);
                        client = tonlib_client_json_create();
                        initialized = false;
                    }
                }
                finally
                {
                    syncRoot.Release();
                }
            }

            if (request.IsStatic)
            {
                return ExecuteInternalStatic(request);
            }

            if (!initialized && request is not Init)
            {
                throw new InvalidOperationException($"Must call {nameof(InitIfNeeded)}() first");
            }

            if (!await syncRoot.WaitAsync(tonOptions.ConcurrencyTimeout))
            {
                throw new TimeoutException("Failed while waiting for semaphore");
            }

            try
            {
                var res = await ExecuteInternalAsync(request);

                if (request is Init)
                {
                    initialized = true;
                }

                return res;
            }
            finally
            {
                syncRoot.Release();
            }
        }

        public decimal ConvertFromNanoTon(long nano)
        {
            return TonUtils.Coins.FromNano(nano);
        }

        public long ConvertToNanoTon(decimal ton)
        {
            return TonUtils.Coins.ToNano(ton);
        }

        [return: NotNullIfNotNull("source")]
        public string? EncodeStringAsBase64(string? source)
        {
            return TonUtils.Text.EncodeAsBase64(source);
        }

        public bool TryDecodeBase64AsString(string? source, [NotNullWhen(true)] out string? result)
        {
            return TonUtils.Text.TryDecodeBase64(source, out result);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        ~TonClient()
        {
            Dispose(disposing: false);
        }

        protected async Task<TResponse> ExecuteInternalAsync<TResponse>(RequestBase<TResponse> request)
            where TResponse : TypeBase
        {
            if (client == null)
            {
                throw new InvalidOperationException("Client not connected");
            }

            if (request.IsStatic)
            {
                throw new InvalidOperationException("This request must be sent as 'static'");
            }

            var reqText = tonOptions.Serializer.Serialize(request);

            if (tonOptions.LogTextLimit > 0 && reqText.Length > tonOptions.LogTextLimit)
            {
                logger.LogDebug("Sending (trimmed from {Length} chars):  {Text}...", reqText.Length, reqText[..tonOptions.LogTextLimit]);
            }
            else
            {
                logger.LogDebug("Sending:  {Text}", reqText);
            }

            tonlib_client_json_send(client.Value, reqText);

            var endOfLoop = DateTimeOffset.UtcNow.Add(request is Sync ? tonOptions.TonClientSyncTimeout : tonOptions.TonClientTimeout);

            while (true)
            {
                var respTextPtr = tonlib_client_json_receive(client.Value, tonOptions.TonLibTimeout.TotalSeconds);
                var respText = Marshal.PtrToStringAnsi(respTextPtr);

                if (string.IsNullOrEmpty(respText))
                {
                    throw new TonClientException(0, "Empty response received");
                }

                if (tonOptions.LogTextLimit > 0 && respText.Length > tonOptions.LogTextLimit)
                {
                    logger.LogDebug("Received (trimmed from {Length} chars): {Text}...", respText.Length, respText[..tonOptions.LogTextLimit]);
                }
                else
                {
                    logger.LogDebug("Received: {Text}", respText);
                }

                var respObj = tonOptions.Serializer.Deserialize(respText);

                if (respObj == null)
                {
                    Deinit();
                    throw new TonClientException(0, "Failed to parse response as Json");
                }

                if (respObj is Error error)
                {
                    if (error.Code == 500)
                    {
                        Deinit();
                    }

                    throw new TonClientException(error.Code, error.Message) { ActualAnswer = error };
                }

                if (respObj is UpdateSyncState uss)
                {
                    if (uss.SyncState is UpdateSyncState.SyncStateDone)
                    {
                        // next 'receive' will give us required data!
                        continue;
                    }

                    if (uss.SyncState is UpdateSyncState.SyncStateInProgress ssip)
                    {
                        SyncStateCurrentSeqno = ssip.CurrentSeqno;

                        if (DateTimeOffset.UtcNow < endOfLoop)
                        {
                            var delay = (ssip.ToSeqno - ssip.CurrentSeqno) < 1000 ? 50 : 500;
                            await Task.Delay(delay);
                            continue;
                        }
                    }

                    Deinit();
                    throw new TonClientException(0, "Failed to wait for sync to complete") { ActualAnswer = uss };
                }

                if (respObj is TResponse resp)
                {
                    return resp;
                }

                Deinit();
                throw new TonClientException(0, "Invalid (unexpected) response type") { ActualAnswer = respObj };
            }
        }

        protected TResponse ExecuteInternalStatic<TResponse>(RequestBase<TResponse> request)
            where TResponse : TypeBase
        {
            if (client == null)
            {
                throw new InvalidOperationException("Client not connected");
            }

            if (!request.IsStatic)
            {
                throw new InvalidOperationException("This request can not be sent as 'static'");
            }

            var reqText = tonOptions.Serializer.Serialize(request);

            if (tonOptions.LogTextLimit > 0 && reqText.Length > tonOptions.LogTextLimit)
            {
                logger.LogDebug("Sending static (trimmed from {Length} chars):  {Text}...", reqText.Length, reqText[..tonOptions.LogTextLimit]);
            }
            else
            {
                logger.LogDebug("Sending static:  {Text}", reqText);
            }

            var respTextPtr = tonlib_client_json_execute(client.Value, reqText);
            var respText = Marshal.PtrToStringAnsi(respTextPtr);

            if (string.IsNullOrEmpty(respText))
            {
                throw new TonClientException(0, "Empty response received");
            }

            if (tonOptions.LogTextLimit > 0 && respText.Length > tonOptions.LogTextLimit)
            {
                logger.LogDebug("Received static (trimmed from {Length} chars): {Text}...", respText.Length, respText[..tonOptions.LogTextLimit]);
            }
            else
            {
                logger.LogDebug("Received static: {Text}", respText);
            }

            var respObj = tonOptions.Serializer.Deserialize(respText);

            if (respObj == null)
            {
                Deinit();
                throw new TonClientException(0, "Failed to parse response as Json");
            }

            if (respObj is Error error)
            {
                if (error.Code == 500)
                {
                    Deinit();
                }

                throw new TonClientException(error.Code, error.Message) { ActualAnswer = error };
            }

            if (respObj is TResponse resp)
            {
                return resp;
            }

            Deinit();
            throw new TonClientException(0, "Invalid (unexpected) response type") { ActualAnswer = respObj };
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects)
                }

                if (client != null)
                {
                    tonlib_client_json_destroy(client.Value);
                    client = null;
                }

                isDisposed = true;
            }
        }
    }
}
