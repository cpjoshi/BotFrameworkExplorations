using AdapativeCardExperiments.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdapativeCardExperiments.Repository
{
    public class KycRepository
    {
        private List<KycRecord> _list = new List<KycRecord>();
        public void Add(KycRecord record)
        {
            lock(this)
            {
                _list.Add(record);
            }
        }

        public IEnumerable<KycRecord> GetRecords(int count)
        {
            return _list.OrderByDescending(x => x.version).Take(count);
        }

    }
}
