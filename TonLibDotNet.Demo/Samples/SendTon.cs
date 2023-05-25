using Microsoft.Extensions.Logging;
using TonLibDotNet.Types.Wallet;
using TonLibDotNet.Types;

namespace TonLibDotNet.Samples
{
    public class SendTon : ISample
    {
        private readonly ITonClient tonClient;
        private readonly ILogger logger;

        public SendTon(ITonClient tonClient, ILogger<SendTon> logger)
        {
            this.tonClient = tonClient ?? throw new ArgumentNullException(nameof(tonClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Run(bool inMainnet)
        {
            if (inMainnet)
            {
                logger.LogWarning("SendTon() sample in Mainnet is disabled for safety reasons. Switch to testnet in Program.cs and try again.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Program.TestMnemonic))
            {
                logger.LogWarning("Actual mnemonic is not set, SendTon() sample is aborted. Put mnemonic phrase in Prograg.cs and try again.");
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

            var inputKey = await tonClient.ImportKey(new ExportedKey(Program.TestMnemonic.Split(' ').ToList()));
            var initialAccountState = new V3InitialAccountState() { PublicKey = inputKey.PublicKey, WalletId = walletId };
            var address = await tonClient.GetAccountAddress(initialAccountState, 0, 0);
            logger.LogDebug("Verifying addresses: expected '{Valid}', got '{Actual}'", Program.TestAddress, address.Value);
            if (Program.TestAddress != address.Value)
            {
                logger.LogError("Address mismatch, aborting. Check mnemonic words and wallet version you are testing with.");
                return;
            }

            // Step 2: Build message and action
            var msg = new Types.Msg.Message(new AccountAddress(Program.TestAddress))
            {
                Data = new Types.Msg.DataText(TonUtils.Text.EncodeAsBase64("Sent using https://github.com/justdmitry/TonLib.NET")),
                Amount = TonUtils.Coins.ToNano(0.01M),
                SendMode = 1,
            };

            var action = new ActionMsg(new List<Types.Msg.Message>() { msg }) { AllowSendToUninited = true };

            // Step 3: create query and send it
            var query = await tonClient.CreateQuery(new InputKeyRegular(inputKey), address, action, TimeSpan.FromMinutes(1), initialAccountState: initialAccountState);

            // wanna know fees before sending?
            var fees = await tonClient.QueryEstimateFees(query.Id);
            logger.LogInformation("Estimated fees (in nanoton): InFwdFee={Value}, FwdFee={Value}, GasFee={Value}, StorageFee={Value}", fees.SourceFees.InFwdFee, fees.SourceFees.FwdFee, fees.SourceFees.GasFee, fees.SourceFees.StorageFee);

            // Send it to network. You dont have TX id or something in response - just poll getTransactions() for your account and wait for new TX.
            _ = await tonClient.QuerySend(query.Id);
            logger.LogInformation("Send OK. Check your account in explorer");
        }
    }
}