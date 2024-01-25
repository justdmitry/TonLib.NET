using System.Diagnostics.CodeAnalysis;

namespace TonLibDotNet.Utils
{
    public class AddressUtils
    {
        public static readonly AddressUtils Instance = new ();

        public const byte BasechainId = AddressValidator.BasechainId;
        public const byte MasterchainId = AddressValidator.MasterchainId;

        /// <summary>
        /// Checks if provided 'address' is valid (including checksum check), return
        /// </summary>
        /// <param name="address">Address to check.</param>
        /// <param name="workchainId">After successful validation contains 'Workchain ID' of address (compare with <see cref="BasechainId"/> or <see cref="MasterchainId"/>).</param>
        /// <param name="bounceable">After successful validation contains 'Bounceable' flag of address.</param>
        /// <param name="testnetOnly">After successful validation contains 'TestnetOnly' flag of address.</param>
        /// <returns>Returns <b>true</b> if address is valid, and <b>false</b> otherwise.</returns>
        /// <exception cref="ArgumentNullException">When 'address' is null or empty string.</exception>
        public bool IsValid([NotNullWhen(true)] string address, out byte workchainId, out bool bounceable, out bool testnetOnly)
        {
            return AddressValidator.TryParseAddress(address, out workchainId, out _, out bounceable, out testnetOnly, out _);
        }

        /// <summary>
        /// Updates address (if needed): sets 'bounceable' flag to required value.
        /// </summary>
        /// <param name="address">Address to convert.</param>
        /// <param name="bounceable">Required value of 'bounceable' flag.</param>
        /// <returns>Returns updated address (with flag changed). Returns original one when flag has already been equal to required value.</returns>
        /// <exception cref="ArgumentException">When 'address' is not valid (<see cref="IsValid"/>).</exception>
        /// <exception cref="ArgumentNullException">When 'address' is null or empty string.</exception>
        public string SetBounceable(string address, bool bounceable)
        {
            if (!AddressValidator.TryParseAddress(address, out var workchainId, out var accountId, out var bounceableOld, out var testnetOnly, out var urlSafe))
            {
                throw new ArgumentException("Invalid address", nameof(address));
            }

            return bounceable == bounceableOld ? address : AddressValidator.MakeAddress(workchainId, accountId, bounceable, testnetOnly, urlSafe);
        }

		/// <summary>
		/// Validates address, and returns same address but with 'bounceable' flag to required value.
		/// </summary>
		/// <param name="address">Address to validate and convert.</param>
		/// <param name="bounceable">Required value of 'bounceable' flag.</param>
		/// <param name="result">Updated address.</param>
		/// <returns>Returns <b>true</b> if source <paramref name="address"/> is valid (and <paramref name="result"/> is set), and <b>false</b> if <paramref name="address"/> is not valid.</returns>
        /// <remarks>When source <paramref name="address"/> already have required 'bounceable' flag value - it will be returned unchanged after validation.</remarks>
		public bool TrySetBounceable(string address, bool bounceable, [NotNullWhen(true)] out string result)
        {
            if (!AddressValidator.TryParseAddress(address, out var workchainId, out var accountId, out var bounceableOld, out var testnetOnly, out var urlSafe))
            {
                result = string.Empty;
                return false;
            }

            result = bounceable == bounceableOld ? address : AddressValidator.MakeAddress(workchainId, accountId, bounceable, testnetOnly, urlSafe);
            return true;
        }
    }
}
