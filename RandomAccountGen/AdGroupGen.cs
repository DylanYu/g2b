using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g2b.RandomAccountGen
{
    using Common;
    using Google.Api.Ads.AdWords.v201509;

    class AdGroupGen
    {

        private const int AddCount = 2;
        private const int DeleteCount = 1;
        private const int UpdateCount = 1;
        private const int ReturnAddCount = 1;
        private const int ReturnUpdateCount = 1;
        private const int ReturnOldCount = 1;

        public static IList<AdGroup> Gen(IList<Campaign> cs)
        {
            var ret = new List<AdGroup>();
            var cids = cs.Select(c => c.id);
            var existingAgs =
                AdWords.GetAdGroups(cids)
                    .OrEmpty()
                    .Where(a => a != null && a.status != AdGroupStatus.REMOVED)
                    .OrderBy(ag => ag.GetHashCode()).ToList();
            Console.Out.WriteLine("get ad groups {0}", existingAgs.Count);
            var toDelete = existingAgs.Take(DeleteCount).ToList();
            var toUpdate = existingAgs.Skip(DeleteCount).Take(UpdateCount).ToList();
            existingAgs.Skip(DeleteCount+UpdateCount).Take(ReturnOldCount).Each(a=>ret.Add(a));
            if (toDelete.Any())
            {
                var deleteRes = AdWords.SetAdGroups(toDelete.Select(a =>
                {
                    Console.Out.WriteLine("delete ad group id:{0},name:{1}, from campaign: {2}", a.id, a.name,
                        a.campaignName);
                    a.status = AdGroupStatus.REMOVED;
                    a.statusSpecified = true;
                    return a;
                }));
                foreach (var e in deleteRes.partialFailureErrors.OrEmpty())
                {
                    Console.Out.WriteLine("delete ad group error:{0}", e.errorString);
                }
            }
            if (toUpdate.Any())
            {
                var updateRes = AdWords.SetAdGroups(toUpdate.Select(a =>
                {
                    var olds = a.status;
                    if (olds == AdGroupStatus.ENABLED)
                    {
                        a.status = AdGroupStatus.PAUSED;
                    }
                    else
                    {
                        a.status=AdGroupStatus.ENABLED;
                    }
                    Console.Out.WriteLine("update ad group id:{0},name:{1}, from campaign: {2}, from status {3} to status {4}", a.id, a.name,
                        a.campaignName, olds,a.status);
                    a.statusSpecified = true;
                    return a;
                }));
                foreach (var e in updateRes.partialFailureErrors.OrEmpty())
                {
                    Console.Out.WriteLine("update ad group error:{0}", e.errorString);
                }
                updateRes.value.Take(ReturnUpdateCount).Each(a=>ret.Add(a));
            }

            var toAdd = Enumerable.Range(0, AddCount).Select(i=>CreateRandom(i,cs)).ToList();
            if (toAdd.Any())
            {
                var addRes = AdWords.AddAdGroups(toAdd);
                foreach (var e in addRes.partialFailureErrors.OrEmpty())
                {
                    Console.Out.WriteLine("add ad group error:{0}", e.errorString);
                }
                foreach (var a in addRes.value)
                {
                    Console.Out.WriteLine("added ad group id:{0},name:{1} in campaign {2}", a.id, a.name, a.campaignName);
                }
                addRes.value.Take(ReturnAddCount).Each(c => ret.Add(c));
            }
            return ret;
        }

        static AdGroup CreateRandom(int i, IList<Campaign> cs)
        {
            var ret = new AdGroup();
            i = i%cs.Count;
            var parent = cs[i];
            ret.campaignId = parent.id;
            ret.name = "AG" + i + "_" + Guid.NewGuid();
            ret.status=AdGroupStatus.PAUSED;
            ret.biddingStrategyConfiguration = new BiddingStrategyConfiguration
            {
                bids = new Bids[] {new CpcBid {bid = new Money {microAmount = 32000000}}}
            };
            return ret;
        }
    }
}
