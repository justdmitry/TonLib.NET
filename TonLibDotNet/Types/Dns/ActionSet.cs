namespace TonLibDotNet.Types.Dns
{
    [TLSchema("dns.actionSet entry:dns.entry = dns.Action")]
    public class ActionSet : Action
    {
        public ActionSet(Entry entry)
        {
            Entry = entry ?? throw new ArgumentNullException(nameof(entry));
        }

        public Entry Entry { get; set; }
    }
}
