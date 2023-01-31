using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TonLibDotNet.Requests;
using TonLibDotNet.Types;

namespace TonLibDotNet
{
    public class TonClient : ITonClient, IDisposable
    {
        protected readonly JsonSerializerOptions jsonOptions = new()
        {
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
            PropertyNamingPolicy = new Utils.JsonSnakeCaseNamingPolicy(),
            WriteIndented = false,
        };

        private readonly ILogger logger;
        private readonly TonOptions tonOptions;

        private readonly object syncRoot = new();

        private IntPtr? client;
        private bool isDisposed;

        public TonClient(ILogger<TonClient> logger, Microsoft.Extensions.Options.IOptions<TonOptions> options)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.tonOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));

            jsonOptions.Converters.Add(new Utils.DateTimeOffsetConverter());
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
            tonOptions.Options.Config.ConfigJson = await httpClient.GetStringAsync(tonOptions.UseMainnet ? tonOptions.ConfigPathMainnet : tonOptions.ConfigPathTestnet).ConfigureAwait(false);

            return Execute(new Init(tonOptions.Options));
        }

        public TResponse Execute<TResponse>(RequestBase<TResponse> request)
            where TResponse: TypeBase
        {
            if (client == null && request is not Init)
            {
                throw new InvalidOperationException($"Must call {nameof(InitIfNeeded)}() first");
            }

            lock (syncRoot)
            {
                if (client == null)
                {
                    tonlib_client_set_verbosity_level(tonOptions.VerbosityLevel);
                    client = tonlib_client_json_create();
                }

                return ExecuteInternal(request);
            }
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

        protected TResponse ExecuteInternal<TResponse>(RequestBase<TResponse> request)
            where TResponse : TypeBase
        {
            if (client == null)
            {
                throw new InvalidOperationException("Client not connected");
            }

            var reqText = JsonSerializer.Serialize((object)request, jsonOptions);

            if (tonOptions.LogTextLimit > 0 && reqText.Length > tonOptions.LogTextLimit)
            {
                logger.LogDebug("Sending (trimmed):  {Text}...", reqText[..tonOptions.LogTextLimit]);
            }
            else
            {
                logger.LogDebug("Sending:  {Text}", reqText);
            }

            tonlib_client_json_send(client.Value, reqText);

            var respTextPtr = tonlib_client_json_receive(client.Value, tonOptions.Timeout.TotalSeconds);
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

            var respObj = System.Text.Json.JsonDocument.Parse(respText);
            if (respObj == null)
            {
                throw new TonClientException(0, "Failed to parse response as Json");
            }

            if (!respObj.RootElement.TryGetProperty("@type", out var val))
            {
                throw new TonClientException(0, "Failed to get @type of response");
            }

            var respObjType = val.GetString();

            if (respObjType == Error.ErrorTypeName)
            {
                var error = respObj.Deserialize<Error>(jsonOptions)!;
                throw new TonClientException(error.Code, error.Message);
            }

            var retVal = respObj.Deserialize<TResponse>(jsonOptions)!;
            if (respObjType != retVal.TypeName)
            {
                var msg = $"Wrong type returned: '{retVal.TypeName}' expected, '{respObjType}' returned";
                throw new TonClientException(0, msg);
            }

            return retVal;
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
