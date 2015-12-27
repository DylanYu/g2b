using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g2b.Sync
{
    using Common;
    using Google.Api.Ads.AdWords.v201509;

    public class AdGroupSync
    {
        public static void Sync(CampaignChangeData c)
        {
            foreach (var ag in c.changedAdGroups.OrEmpty())
            {
                switch (ag.adGroupChangeStatus)
                {
                    case ChangeStatus.NEW:
                        Console.Out.WriteLine("new ad group not supported");
                        break;
                    case ChangeStatus.FIELDS_UNCHANGED:
                        break;
                    case ChangeStatus.FIELDS_CHANGED:
                        OnAdGroupChange(c, ag);
                        break;
                }
                KeywordSync.Sync(c, ag);
            }
        }

        static void OnAdGroupChange(CampaignChangeData c, AdGroupChangeData ag)
        {
            Console.Out.WriteLine("update ad group not supported");
        }
    }
}
