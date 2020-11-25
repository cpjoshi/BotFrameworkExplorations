using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdapativeCardExperiments.Authentication
{
    public class AzureADOptions
    {
        public string Instance { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
