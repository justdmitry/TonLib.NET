using System.Text.Json.Serialization;

namespace TonLibDotNet.Types
{
    [TLSchema("actionDns actions:vector<dns.Action> = Action")]
    public class ActionDns : Action
    {
        [JsonConstructor]
        public ActionDns(List<Dns.Action> actions)
        {
            Actions = actions ?? throw new ArgumentNullException(nameof(actions));
        }

        public ActionDns(params Dns.Action[] actions)
        {
            Actions = actions.ToList();
        }

        public List<Dns.Action> Actions { get; set; }
    }
}
