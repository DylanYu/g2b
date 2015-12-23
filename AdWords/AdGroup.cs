using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g2b
{
    using Google.Api.Ads.AdWords.Lib;
    using Google.Api.Ads.AdWords.v201509;

    partial class AdWords
    {

        private static readonly Lazy<AdGroupService> AdGroupService =
                    new Lazy<AdGroupService>(
                        () => (AdGroupService)User.Value.GetService(AdWordsService.v201509.AdGroupService));


        public static AdGroup[] GetAdGroups(IEnumerable<long> cids)
        {
            var str = cids.Select(i => i.ToString()).ToArray();
            var s = new Selector
            {
                fields = new[] { "Id", "Name", "Status", "CampaignId", "CampaignName", "CpcBid", "Settings" },
                predicates =
                    new[]
                    {
                        new Predicate
                        {
                            field = "CampaignId",
                            @operator = PredicateOperator.IN,
                            operatorSpecified = true,
                            values = str
                        }
                    }
            };
            return AdGroupService.Value.get(s).entries;
        }

        public static AdGroupReturnValue SetAdGroups(IEnumerable<AdGroup> ags)
        {
            return
                AdGroupService.Value.mutate(
                    ags.Select(ag => new AdGroupOperation { operand = ag, @operator = Operator.SET }).ToArray());
        }

        public static AdGroupReturnValue AddAdGroups(IList<AdGroup> ags)
        {
            return AdGroupService.Value.mutate(
                ags.Select(ag => new AdGroupOperation { operand = ag, @operator = Operator.ADD }).ToArray());
        }
    }
}
