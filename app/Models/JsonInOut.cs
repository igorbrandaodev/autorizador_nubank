using System.Collections.Generic;

namespace authorize.Models
{
    public class JsonInOut
    {
        public Account account { get; set; }
        public Transaction transaction { get; set; }
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
    }
}
