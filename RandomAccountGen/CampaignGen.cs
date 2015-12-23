using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g2b.RandomAccountGen
{
    using Google.Api.Ads.AdWords.v201509;

    public class CampaignGen
    {
        private const int AddCount = 3;
        private const int DeleteCount = 0;
        private const int UpdateCount = 0;

        public static void Gen()
        {
            var existingCampaings = AdWords.GetCampaigns();
            var ls = existingCampaings.Where(c => c.status != CampaignStatus.REMOVED).OrderBy(c => c.GetHashCode());
            var toDelete = ls.Take(DeleteCount).ToList();
            var toUpdate = ls.Take(UpdateCount).ToList();
            if (toDelete.Any())
            {
                var deleteRes = AdWords.SetCampaigns(toDelete.Select(c =>
                {
                    Console.Out.WriteLine("delete campaign id:{0},name:{1}", c.id, c.name);
                    c.status = CampaignStatus.REMOVED;
                    c.statusSpecified = true;
                    return c;
                }));
                if (deleteRes.partialFailureErrors != null)
                    foreach (var e in deleteRes.partialFailureErrors)
                    {
                        Console.Out.WriteLine("delete campaign error:{0}", e.errorString);
                    }
            }
            if (toUpdate.Any())
            {
                var updateRes = AdWords.SetCampaigns(toUpdate.Select(c =>
                {
                    var oldbudget = c.budget.amount.microAmount;
                    c.budget.amount.microAmount += 1000000;
                    if (c.budget.amount.microAmount > 50000000) c.budget.amount.microAmount = 10000000;
                    c.budget.amount.microAmountSpecified = true;
                    Console.Out.WriteLine("udpate campaign id:{0},name:{1},budget from:{2},to{3}", c.id, c.name,
                        oldbudget,
                        c.budget.amount.microAmount);
                    return c;
                }));
                if (updateRes.partialFailureErrors != null)
                    foreach (var e in updateRes.partialFailureErrors)
                    {
                        Console.Out.WriteLine("update campaign error:{0}", e.errorString);
                    }
            }
            var toAdd = Enumerable.Range(0, AddCount).Select(CreateRandom).ToList();
            if (toAdd.Any())
            {
                var addRes = AdWords.AddCampaigns(toAdd);
                if (addRes.partialFailureErrors != null)
                    foreach (var e in addRes.partialFailureErrors)
                    {
                        Console.Out.WriteLine("add campaign error:{0}", e.errorString);
                    }
                foreach (var campaign in addRes.value)
                {
                    Console.Out.WriteLine("added campaign id:{0},name:{1}", campaign.id, campaign.name);
                }
            }
        }

        static Campaign CreateRandom(int i)
        {
            Campaign campaign = new Campaign();
            campaign.name = "My first campaign "+i+"_"+Guid.NewGuid();
            campaign.status = CampaignStatus.PAUSED;
            campaign.advertisingChannelType = AdvertisingChannelType.SEARCH;

            BiddingStrategyConfiguration biddingConfig = new BiddingStrategyConfiguration();
            biddingConfig.biddingStrategyType = BiddingStrategyType.MANUAL_CPC;
            campaign.biddingStrategyConfiguration = biddingConfig;

            campaign.budget = new Budget
            {
                name = "budget for "+ campaign.name,
                period = BudgetBudgetPeriod.DAILY,
                deliveryMethod = BudgetBudgetDeliveryMethod.STANDARD,
                amount = new Money {microAmount = 12000000 + i * 1000000 }
            };
            
            // Set the campaign network options.
            campaign.networkSetting = new NetworkSetting();
            campaign.networkSetting.targetGoogleSearch = true;
            campaign.networkSetting.targetSearchNetwork = true;
            campaign.networkSetting.targetContentNetwork = false;
            campaign.networkSetting.targetPartnerSearchNetwork = false;

            // Set the campaign settings for Advanced location options.
            GeoTargetTypeSetting geoSetting = new GeoTargetTypeSetting();
            geoSetting.positiveGeoTargetType = GeoTargetTypeSettingPositiveGeoTargetType.DONT_CARE;
            geoSetting.negativeGeoTargetType = GeoTargetTypeSettingNegativeGeoTargetType.DONT_CARE;

            campaign.settings = new Setting[] { geoSetting };
            return campaign;
        }
    }
}
