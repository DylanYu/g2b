using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g2b.Sync
{
    using Common;

    public class CampaignSync
    {
        public static void Sync(string t)
        {
            var acs = AdWords.GetCampaigns();
            var delta = AdWords.GetSyncData(t, acs);
            //do sync for all entities
            if (!string.IsNullOrEmpty(delta.lastChangeTimestamp))
                LastSync.Set(delta.lastChangeTimestamp);
        }
    }
}
