using Newtonsoft.Json;
using System.Collections.Generic;

namespace authorize.Models
{
    public class JsonInOut
    {
        public Account account { get; set; }
        public Transaction transaction { get; set; }

        [JsonProperty("allow-list")]
        public AllowList allow_list { get; set; }
        public List<string> violations { get; set; }

        public JsonInOut(Account account, List<string> violations)
        {
            this.account = account;
            this.violations = violations;
        }

        public bool ShouldSerializetransaction()
        {
            return false;
        }

        public bool ShouldSerializeallow_list()
        {
            return false;
        }
    }
}
