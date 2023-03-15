namespace TonLibDotNet.Types
{
    [TLSchema("actionDns actions:vector<dns.Action> = Action")]
    public class ActionDns : Action
    {
        public ActionDns(List<Dns.Action> actions)
        {
            Actions = actions ?? throw new ArgumentNullException(nameof(actions));
        }

        public List<Dns.Action> Actions { get; set; }
    }
}
