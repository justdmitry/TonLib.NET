using Microsoft.Extensions.Logging;
using TonLibDotNet.Types.Wallet;
using TonLibDotNet.Types;
using Microsoft.Extensions.Options;

namespace TonLibDotNet
{
    public class SendTon
    {
        private readonly ITonClient tonClient;
        private readonly ILogger logger;
        private readonly TonOptions tonOptions;

        public SendTon(ITonClient tonClient, ILogger<SendTon> logger, IOptions<TonOptions> tonOptions)
        {
            this.tonClient = tonClient ?? throw new ArgumentNullException(nameof(tonClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.tonOptions = tonOptions?.Value ?? throw new ArgumentNullException(nameof(tonOptions));
        }

        public async Task Run()
        {
            if (tonOptions.UseMainnet)
            {
                logger.LogWarning("Sending TON demo in Mainnet is disabled for safety reasons");
                return;
            }

            if (Program.TestnetAccountToSendFromMnemonic[0] == "word1")
            {
                logger.LogWarning("Sample mnemonic is not set. Sending TON demo is skipped.");
                return;
            }

            await tonClient.InitIfNeeded();

            /*
             * See also https://ton.org/docs/develop/dapps/asset-processing/#deploying-wallet
             *      and https://ton.org/docs/develop/dapps/asset-processing/#sending-payments
             */

            // Step 1: Import key and find your address

            // Docs says you should use value from network config.
            var walletId = tonClient.OptionsInfo.ConfigInfo.DefaultWalletId;

            // Surprise! Even for testnet, wallet.ton.org uses mainnet value :(
            walletId = 698983191;

            var inputKey = await tonClient.ImportKey(new ExportedKey(Program.TestnetAccountToSendFromMnemonic.ToList()));
            var initialAccountState = new V3InitialAccountState() { PublicKey = inputKey.PublicKey, WalletId = walletId };
            var address = await tonClient.GetAccountAddress(initialAccountState, 0, 0);
            logger.LogDebug("Verifying addresses: expected '{Valid}', got '{Actual}'", Program.TestnetAccountToSendFromAddress, address.Value);
            if (Program.TestnetAccountToSendFromAddress != address.Value)
            {
                logger.LogError("Address mismatch. Aborting.");
                return;
            }

            // Step 2: Build message and action
            var msg = new Types.Msg.Message(new AccountAddress(Program.TestnetAccountToSendFromAddress))
            {
                Data = new Types.Msg.DataText(tonClient.EncodeStringAsBase64("Sent using https://github.com/justdmitry/TonLib.NET")),
                Amount = tonClient.ConvertToNanoTon(0.01M),
                SendMode = 1,
            };

            var action = new Types.ActionMsg(new List<Types.Msg.Message>() { msg }) { AllowSendToUninited = true };

            // Step 3: create query and send it
            var query = await tonClient.CreateQuery(new InputKeyRegular(inputKey), address, action, TimeSpan.FromMinutes(1), initialAccountState: initialAccountState);

            // wanna know fees before sending?
            var fees = await tonClient.QueryEstimateFees(query.Id);

            // Send it to network. You dont have TX id or something in response - just poll getTransactions() for your account and wait for new TX.
            _ = await tonClient.QuerySend(query.Id);
        }
    }
}