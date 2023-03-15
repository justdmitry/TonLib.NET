using TonLibDotNet.Requests.Dns;
using TonLibDotNet.Types;
using TonLibDotNet.Types.Dns;

namespace TonLibDotNet
{
    public static class TonClientDnsExtensions
    {
        /// <summary>
        /// Performs DNS lookup - recursively resolves .ton domain to ADNL address.
        /// </summary>
        /// <param name="client">ITonClient instance.</param>
        /// <param name="name">Name (part of name) of TON domain to resolve.</param>
        /// <param name="accountAddress">Address of resolver you want to ask.</param>
        /// <param name="ttl">Number of "recursive hops" that are executed before returning 'NextResolver' without querying it.</param>
        /// <param name="category">Required category. If the category is null or <see cref="TypeBase.Int256_AllZeroes">zero</see> then all categories are requested.</param>
        /// <seealso href="https://github.com/ton-blockchain/ton/blob/v2023.01/tonlib/tonlib/TonlibClient.cpp#L4174" />
        /// <seealso href="https://github.com/ton-blockchain/TEPs/blob/master/text/0081-dns-standard.md">TEP-81 TON DNS Standard</seealso>
        public static Task<Resolved> DnsResolve(this ITonClient client, string name, AccountAddress? accountAddress = null, int? ttl = 99, string? category = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return client.Execute(new Resolve { AccountAddress = accountAddress, Name = name, Category = category, Ttl = ttl });
        }
    }
}
