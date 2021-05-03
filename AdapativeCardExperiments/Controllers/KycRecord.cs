using System;

namespace AdapativeCardExperiments.Controllers
{
    public class KycRecord
    {
        public string Id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public DateTime birthday { get; set; }
        public long version { get; set; }
    }
}