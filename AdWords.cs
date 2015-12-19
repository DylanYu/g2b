using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g2b
{
    using System.IO;
    using Google.Api.Ads.AdWords.Lib;
    using Google.Api.Ads.AdWords.v201509;

    class AdWords
    {
        public static void GetCampaigns()
        {
            var user = new AdWordsUser();
            AdWordsAppConfig config = (AdWordsAppConfig)user.Config;
            config.DeveloperToken = File.ReadAllText(@"d:\devtoken");
            var campaignService = (CampaignService)user.GetService(AdWordsService.v201509.CampaignService);
            var sel0 = new Selector
            {
                fields = new[] { "Id", "Name" },
            };
            var page = campaignService.get(sel0);
            foreach (var c in page.entries)
            {
                Console.Out.WriteLine(c.id);
                Console.Out.WriteLine(c.name);
            }
        }
    }
}
