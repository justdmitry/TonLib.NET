﻿using Microsoft.Extensions.Logging;
using TonLibDotNet.Internal;
using TonLibDotNet.Requests;
using TonLibDotNet.Types;

namespace TonLibDotNet.Samples
{
    public class LibraryExtensibility : ISample
    {
        private readonly ITonClient tonClient;
        private readonly ILogger logger;

        public LibraryExtensibility(ITonClient tonClient, ILogger<LibraryExtensibility> logger)
        {
            this.tonClient = tonClient ?? throw new ArgumentNullException(nameof(tonClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Run(bool inMainnet)
        {
            await tonClient.InitIfNeeded();

            // This will fail because TonLib does not have this method.
            // This is just a demo how you can add new types/requests without waiting for new package release.
            // To make this happen in your app - call TonClient.RegisterAssembly() early
            try
            {
                await tonClient.Execute(new ExtensibilityDemoRequest() { Continent = "Antarctica" });
            }
            catch (TonClientException ex)
            {
                logger.LogWarning(ex, "Exception ignored");
                // Ignore
            }
        }

        [TLSchema("hello.world = hello.World")]
        public class ExtensibilityDemoRequest : RequestBase<Ok>
        {
            public string Continent { get; set; } = string.Empty;
        }
    }
}
