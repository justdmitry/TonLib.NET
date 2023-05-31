using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TonLibDotNet.Types;

namespace TonLibDotNet
{
    public abstract class TonClientBaseFixture : IDisposable
    {
        private readonly TonClient tonClient;

        protected TonClientBaseFixture(bool useMainnet)
        {
            var logger = new Mock<ILogger<TonClient>>(MockBehavior.Loose);
            var options = new Mock<IOptions<TonOptions>>(MockBehavior.Strict);
            options
                .SetupGet(x => x.Value)
                .Returns(() =>
                {
                    var o = new TonOptions() { UseMainnet = useMainnet };
                    o.Options.KeystoreType = new KeyStoreTypeInMemory();
                    return o;
                });
            tonClient = new TonClient(logger.Object, options.Object);
        }

        public async Task<ITonClient> GetTonClient()
        {
            await tonClient.InitIfNeeded();
            await tonClient.Sync();
            return tonClient;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            tonClient.Dispose();
        }
    }
}
