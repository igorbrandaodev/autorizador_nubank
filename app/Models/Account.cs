using Newtonsoft.Json;
using System.Collections.Generic;

namespace authorize.Models
{
    public class Account
    {
        [JsonProperty("active-card")]
        public bool active_card { get; }

        [JsonProperty("available-limit")]
        public int available_limit { get; }

        [JsonIgnore]
        public List<Transaction> history { get; }

        public Account(bool active_card, int available_limit, List<Transaction> history)
        {
            this.active_card = active_card;
            this.available_limit = available_limit;
            this.history = history;
        }
    }
}
