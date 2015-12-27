using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g2b.Sync
{
    using Common;
    using Google.Api.Ads.AdWords.v201509;

    public class CampaignSync
    {
        public static void Sync(string t)
        {
            var acs = AdWords.GetCampaigns();
            var delta = AdWords.GetSyncData(t, acs);
            foreach (var campaign in delta.changedCampaigns)
            {
                switch (campaign.campaignChangeStatus)
                {
                    case ChangeStatus.NEW:
                        Console.Out.WriteLine("new campaign and child not supported");
                        break;
                    case ChangeStatus.FIELDS_UNCHANGED:
                        break;
                    case ChangeStatus.FIELDS_CHANGED:
                        OnCampaignChange(campaign);
                        break;
                }
                AdGroupSync.Sync(campaign);
            }
            if (!string.IsNullOrEmpty(delta.lastChangeTimestamp))
                LastSync.Set(delta.lastChangeTimestamp);
        }

        static void OnCampaignChange(CampaignChangeData c)
        {
            Console.Out.WriteLine("update campaign not supported");
        }
    }
}
