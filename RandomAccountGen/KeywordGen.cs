using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g2b.RandomAccountGen
{
    using Common;
    using Google.Api.Ads.AdWords.v201509;

    public class KeywordGen
    {
        private const int AddCount = 4;
        private const int DeleteCount = 1;
        private const int UpdateCount = 1;

        public static void Gen(IList<AdGroup> ags)
        {
            var agids = ags.Select(a => a.id);
            var kws =
                AdWords.GetKeywords(agids)
                    .Where(k => k.Ac.userStatus != UserStatus.REMOVED)
                    .OrderBy(k => k.GetHashCode())
                    .ToList();
            Console.Out.WriteLine("get keywords {0}", kws.Count);
            var toDelete = kws.Take(DeleteCount).ToList();
            var toUpdate = kws.Skip(DeleteCount).Take(UpdateCount).ToList();
            if (toDelete.Any())
            {
                var deleteRes = AdWords.SetKeywords(toDelete.Select(k =>
                {
                    Console.Out.WriteLine("delete keyword id:{0} text:{1} mt:{2} agid:{3}", k.Ac.criterion.id,
                        k.Keyword.text,
                        k.Keyword.matchType, k.Ac.adGroupId);
                    return k;
                }), true);

                foreach (var e in deleteRes.partialFailureErrors.OrEmpty())
                {
                    Console.Out.WriteLine("delete keyword error:{0}", e.errorString);
                }
            }

            if (toUpdate.Any())
            {
                var updateRes = AdWords.SetKeywords(toUpdate.Select(k =>
                {
                    var bid = k.Ac.biddingStrategyConfiguration.bids[0] as CpcBid;
                    if (bid != null)
                    {
                        bid.bid.microAmount += 1000000;
                    }
                    else
                    {
                        throw new Exception("bid not cpc");
                    }
                    Console.Out.WriteLine("update keyword id:{0} text:{1} mt:{2} agid:{3} nb:{4}", k.Ac.criterion.id,
                        k.Keyword.text,
                        k.Keyword.matchType, k.Ac.adGroupId, bid.bid.microAmount);
                    return k;
                }));
                foreach (var e in updateRes.partialFailureErrors.OrEmpty())
                {
                    Console.Out.WriteLine("update keyword error:{0}", e.errorString);
                }
            }
            var toAdd = Enumerable.Range(0, AddCount).Select(i => CreateRandom(i, ags)).ToList();

            if (toAdd.Any())
            {
                var addRes = AdWords.AddKeywords(toAdd);
                foreach (var e in addRes.partialFailureErrors.OrEmpty())
                {
                    Console.Out.WriteLine("add kw error:{0}", e.errorString);
                }
                foreach (var a in addRes.value)
                {
                    var kw = a.criterion as Keyword;
                    if (kw == null) continue;
                    Console.Out.WriteLine("added kw id:{0},text:{1},mt:{2} in ad group id {3}", kw.id, kw.text,
                        kw.matchType, a.adGroupId);
                }
            }

        }

        static BiddableAdGroupCriterion CreateRandom(int i, IList<AdGroup> ags)
        {
            var k = new BiddableAdGroupCriterion();
            i = i%ags.Count;
            var parent = ags[i];
            k.adGroupId = parent.id;
            k.biddingStrategyConfiguration = new BiddingStrategyConfiguration
            {
                bids = new Bids[] { new CpcBid { bid = new Money { microAmount = 11000000 } } }
            };
            k.criterion = new Keyword {matchType = KeywordMatchType.BROAD, text = "kw " + i + "_" + Guid.NewGuid()};
            return k;
        }
    }
}
