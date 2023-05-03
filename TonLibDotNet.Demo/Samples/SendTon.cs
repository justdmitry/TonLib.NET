using Microsoft.Extensions.Logging;
using TonLibDotNet.Types.Wallet;
using TonLibDotNet.Types;
using Microsoft.Extensions.Options;

namespace TonLibDotNet.Samples
{
    public class SendTon
    {
        // You need actual mnemonic and address with some coins to test sending.
        // Double check that you are using testnet!!!
        private const string TestAddress = "EQAkEWzRLi1sw9AlaGDDzPvk2_F20hjpTjlvsjQqYawVmdT0";

        private const string TestMnemonic = "word1 word2 word3 word4 word5 word6 word7 word8 word9 word10 word11 word12 word13 word14 word15 word16 word17 word18 word19 word20 word21 word22 word23 word24";

        private readonly ITonClient tonClient;
        private readonly ILogger logger;

        public SendTon(ITonClient tonClient, ILogger<SendTon> logger)
        {
            this.tonClient = tonClient ?? throw new ArgumentNullException(nameof(tonClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Run(bool useMainnet)
        {
            if (useMainnet)
            {
                logger.LogWarning("SendTon() demo in Mainnet is disabled for safety reasons");
                return;
            }

            if (TestMnemonic.StartsWith("word1 "))
            {
                logger.LogWarning("Actual mnemonic is not set. SendTon() demo is skipped.");
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

            var inputKey = await tonClient.ImportKey(new ExportedKey(TestMnemonic.Split(' ').ToList()));
            var initialAccountState = new V3InitialAccountState() { PublicKey = inputKey.PublicKey, WalletId = walletId };
            var address = await tonClient.GetAccountAddress(initialAccountState, 0, 0);
            logger.LogDebug("Verifying addresses: expected '{Valid}', got '{Actual}'", TestAddress, address.Value);
            if (TestAddress != address.Value)
            {
                logger.LogError("Address mismatch, aborting. Check mnemonic words and wallet version you are testing with.");
                return;
            }

            // Step 2: Build message and action
            var msg = new Types.Msg.Message(new AccountAddress(TestAddress))
            {
                Data = new Types.Msg.DataText(tonClient.EncodeStringAsBase64("Sent using https://github.com/justdmitry/TonLib.NET")),
                Amount = tonClient.ConvertToNanoTon(0.01M),
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