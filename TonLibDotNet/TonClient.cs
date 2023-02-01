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
        private bool isDisposed;

        public TonClient(ILogger<TonClient> logger, Microsoft.Extensions.Options.IOptions<TonOptions> options)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.tonOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        [DllImport("tonlibjson")]
        private static extern IntPtr tonlib_client_json_create();

        [DllImport("tonlibjson")]
        private static extern void tonlib_client_json_destroy(IntPtr client);

        [DllImport("tonlibjson")]
        private static extern void tonlib_client_set_verbosity_level(int level);

        [DllImport("tonlibjson", CharSet = CharSet.Ansi)]
        private static extern IntPtr tonlib_client_json_execute(IntPtr client, string request);

        [DllImport("tonlibjson", CharSet = CharSet.Ansi)]
        private static extern void tonlib_client_json_send(IntPtr client, string request);

        [DllImport("tonlibjson")]
        private static extern IntPtr tonlib_client_json_receive(IntPtr client, double timeout);

        public async Task<OptionsInfo?> InitIfNeeded()
        {
            if (client != null)
            {
                return null;
            }

            var httpClient = new HttpClient();
            var fullConfig = await httpClient.GetStringAsync(tonOptions.UseMainnet ? tonOptions.ConfigPathMainnet : tonOptions.ConfigPathTestnet).ConfigureAwait(false);

            var jdoc = JsonNode.Parse(fullConfig);
            var servers = jdoc["liteservers"].AsArray();
            var choosen = tonOptions.LiteServerSelector(servers);
            servers.Clear();
            servers.Add(choosen);
            logger.LogInformation("LiteServer choosen: ip={IP}, port={Port}, key={Key}", choosen["ip"], choosen["port"], choosen["id"]?["key"]);

            tonOptions.Options.Config.ConfigJson = jdoc.ToJsonString();

            return await Execute(new Init(tonOptions.Options));
        }

        public async Task<TResponse> Execute<TResponse>(RequestBase<TResponse> request)
            where TResponse: TypeBase
        {
            if (client == null && request is not Init)
            {
                throw new InvalidOperationException($"Must call {nameof(InitIfNeeded)}() first");
            }

            if (await syncRoot.WaitAsync(tonOptions.TonClientTimeout))
            {
                try
                {
                    if (client == null)
                    {
                        tonlib_client_set_verbosity_level(tonOptions.VerbosityLevel);
                        client = tonlib_client_json_create();
                    }

                    return await ExecuteInternal(request);
                }
                finally
                {
                    syncRoot.Release();
                }
            }

            throw new TimeoutException("Failed while waiting for semaphore");
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

        protected async Task<TResponse> ExecuteInternal<TResponse>(RequestBase<TResponse> request)
            where TResponse : TypeBase
        {
            if (client == null)
            {
                throw new InvalidOperationException("Client not connected");
            }

            var reqText = tonOptions.Serializer.Serialize(request);

            if (tonOptions.LogTextLimit > 0 && reqText.Length > tonOptions.LogTextLimit)
            {
                logger.LogDebug("Sending (trimmed):  {Text}...", reqText[..tonOptions.LogTextLimit]);
            }
            else
            {
                logger.LogDebug("Sending:  {Text}", reqText);
            }

            tonlib_client_json_send(client.Value, reqText);

            var endOfLoop = DateTimeOffset.UtcNow.Add(tonOptions.TonClientTimeout);

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
                    logger.LogDebug("Recieved (trimmed): {Text}...", respText[..tonOptions.LogTextLimit]);
                }
                else
                {
                    logger.LogDebug("Recieved: {Text}", respText);
                }

                var respObj = tonOptions.Serializer.Deserialize(respText);

                if (respObj == null)
                {
                    throw new TonClientException(0, "Failed to parse response as Json");
                }

                if (respObj is Error error)
                {
                    throw new TonClientException(error.Code, error.Message) { ActualAnswer = error };
                }

                if (respObj is UpdateSyncState uss)
                {
                    if (uss.SyncState is UpdateSyncState.SyncStateDone)
                    {
                        // next 'receive' will give us required data!
                        continue;
                    }

                    if (DateTimeOffset.UtcNow < endOfLoop && uss.SyncState is UpdateSyncState.SyncStateInProgress ssip)
                    {
                        var delay = (ssip.ToSeqno - ssip.CurrentSeqno) < 1000 ? 50 : 500;
                        await Task.Delay(delay);
                        continue;
                    }

                    throw new TonClientException(0, "Failed to wait for sync to complete") { ActualAnswer = uss };
                }

                if (respObj is TResponse resp)
                {
                    return resp;
                }

                throw new TonClientException(0, "Invalid (unexpected) response type") { ActualAnswer = respObj };
            }
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
