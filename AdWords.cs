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
        private static readonly Lazy<AdWordsUser> User=new Lazy<AdWordsUser>(() =>
        {
            var user = new AdWordsUser();
            AdWordsAppConfig config = (AdWordsAppConfig)user.Config;
            config.DeveloperToken = File.ReadAllText(@"d:\devtoken");
            return user;
        });

        private static readonly Lazy<CampaignService> CampaignService =
            new Lazy<CampaignService>(
                () => (CampaignService) User.Value.GetService(AdWordsService.v201509.CampaignService));

        private static readonly Lazy<BudgetService> BudgetService = new Lazy<BudgetService>(() =>
            (BudgetService) User.Value.GetService(AdWordsService.v201509.BudgetService));

        private static readonly Lazy<AdGroupService> AdGroupService =
            new Lazy<AdGroupService>(
                () => (AdGroupService) User.Value.GetService(AdWordsService.v201509.AdGroupAdService)); 

        public static Campaign[] GetCampaigns()
        {
            var sel0 = new Selector
            {
                fields = new[] { "Id", "Name", "Amount", "Status" },
            };
            return CampaignService.Value.get(sel0).entries;
        }

        public static CampaignReturnValue SetCampaigns(IEnumerable<Campaign> cs)
        {
            return CampaignService.Value.mutate(
                cs.Select(c => new CampaignOperation {operand = c, @operator = Operator.SET}).ToArray());
        }

        public static CampaignReturnValue AddCampaigns(IList<Campaign> cs)
        {
            var r = BudgetService.Value.mutate(
                cs.Select(c => new BudgetOperation {@operator = Operator.ADD, operand = c.budget}).ToArray());
            for (var i = 0; i < r.value.Length; i++)
            {
                cs[i].budget.budgetId = r.value[i].budgetId;
            }
            return CampaignService.Value.mutate(
                cs.Select(c => new CampaignOperation {operand = c, @operator = Operator.ADD}).ToArray());
        }

        public static AdGroup[] GetAdGroups(IEnumerable<long> cids)
        {
            var str = cids.Select(i => i.ToString()).ToArray();
            var s = new Selector
            {
                fields = new[] {"Id", "Name", "Status", "CampaignId", "CampaignName", "CpcBid", "Settings" },
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
                    ags.Select(ag => new AdGroupOperation {operand = ag, @operator = Operator.SET}).ToArray());
        }

        public static AdGroupReturnValue AddAdGroups(IList<AdGroup> ags)
        {
            return AdGroupService.Value.mutate(
                ags.Select(ag => new AdGroupOperation { operand = ag, @operator = Operator.ADD }).ToArray());
        }
    }
}
