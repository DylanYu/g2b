using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g2b
{
    using System.IO;
    using Common;
    using Google.Api.Ads.AdWords.Lib;
    using Google.Api.Ads.AdWords.v201509;

    partial class AdWords
    {
        private static readonly Lazy<AdWordsUser> User = new Lazy<AdWordsUser>(() =>
        {
            var user = new AdWordsUser();
            AdWordsAppConfig config = (AdWordsAppConfig) user.Config;
            config.DeveloperToken = File.ReadAllText(@"d:\devtoken");
            return user;
        });

        public static readonly Lazy<CustomerSyncService> SyncService = new Lazy<CustomerSyncService>
            (() => (CustomerSyncService) User.Value.GetService(AdWordsService.v201509.CustomerSyncService)
            );


        public static CustomerChangeData GetSyncData(string startTime, Campaign[] campaigns)
        {
            var ids = campaigns.Where(c => c.status != CampaignStatus.REMOVED).Select(c => c.id).ToArray();
            var maxt = LastSync.Format(DateTime.UtcNow);
            Console.Out.WriteLine("startTime:{0} maxt:{1}", startTime, maxt);
            var sel = new CustomerSyncSelector
            {
                dateTimeRange = new DateTimeRange { min = startTime, max = maxt },
                campaignIds = ids,
            };
            //try catch if too many delta exception, reduce maxt and try again
            var delta = SyncService.Value.get(sel);
            DumpDelta(delta);
            return delta;
        }

        private static void DumpDelta(CustomerChangeData delta)
        {
            var sb = new StringBuilder();
            sb.AppendLine("begin dump delta");
            sb.AppendFormat("lastChangeTimestamp:{0}\n", delta.lastChangeTimestamp);
            sb.AppendFormat("changedCampaigns len:{0}\n", delta.changedCampaigns.OrEmpty().Count());
            foreach (var c in delta.changedCampaigns.OrEmpty())
            {
                sb.AppendFormat("cid:{0} change:{1} # changed ad groups:{2}\n", c.campaignId, c.campaignChangeStatus,
                    c.changedAdGroups.OrEmpty().Count());
                foreach (var ag in c.changedAdGroups.OrEmpty())
                {
                    sb.AppendFormat("    agid:{0} change:{1} # changed critiera:{2} removed critiera:{3}\n", ag.adGroupId,
                        ag.adGroupChangeStatus, ag.changedCriteria.OrEmpty().Count(), ag.removedCriteria.OrEmpty().Count());
                    sb.AppendFormat("    changed critiera:{0}\n",
                        string.Join(",", ag.changedCriteria.OrEmpty().Select(i => i.ToString())));
                    sb.AppendFormat("    removed critiera:{0}\n",
                        string.Join(",", ag.removedCriteria.OrEmpty().Select(i => i.ToString())));
                }
            }
            sb.AppendLine("end dump delta");
            Console.Out.WriteLine(sb);
        }
    }
}
