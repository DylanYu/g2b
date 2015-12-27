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

        private static readonly Lazy<CampaignService> CampaignService =
            new Lazy<CampaignService>(
                () => (CampaignService)User.Value.GetService(AdWordsService.v201509.CampaignService));

        private static readonly Lazy<BudgetService> BudgetService = new Lazy<BudgetService>(() =>
            (BudgetService)User.Value.GetService(AdWordsService.v201509.BudgetService));

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
                cs.Select(c => new CampaignOperation { operand = c, @operator = Operator.SET }).ToArray());
        }

        public static CampaignReturnValue AddCampaigns(IList<Campaign> cs)
        {
            var r = BudgetService.Value.mutate(
                cs.Select(c => new BudgetOperation { @operator = Operator.ADD, operand = c.budget }).ToArray());
            for (var i = 0; i < r.value.Length; i++)
            {
                cs[i].budget.budgetId = r.value[i].budgetId;
            }
            return CampaignService.Value.mutate(
                cs.Select(c => new CampaignOperation { operand = c, @operator = Operator.ADD }).ToArray());
        }

    }
}
