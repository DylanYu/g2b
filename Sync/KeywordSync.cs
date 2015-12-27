using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g2b.Sync
{
    using System.IO;
    using Common;
    using Google.Api.Ads.AdWords.v201509;
    using IdMap;
    using Transform;

    public class KeywordSync
    {
        public static void Sync(CampaignChangeData c, AdGroupChangeData ag)
        {
            //load keyword map
            var agMapFp = AdGroupMap.GetPath(c.campaignId);
            if (!File.Exists(agMapFp))
            {
                Console.Out.WriteLine("ERR: agMapFp lost");
                return;
            }
            var kwMapFp = KeywordMap.GetPath(c.campaignId, ag.adGroupId);
            if (!File.Exists(kwMapFp))
            {
                Console.Out.WriteLine("ERR: kwMapFp lost");
                return;
            }
            var agMap = Util.ReadMap(agMapFp);
            if (!agMap.ContainsKey(ag.adGroupId))
            {
                Console.Out.WriteLine("ERR: ag.adGroupId lost in agMap");
                return;
            }
            var bagid = agMap[ag.adGroupId];
            var keywordMap = Util.ReadMap(kwMapFp);

            var toDelete = new List<long>();
            var toUpdate = new List<Microsoft.BingAds.CampaignManagement.Keyword>();
            var toAdd = new List<Listing>();

            //delete
            foreach (var akwid in ag.removedCriteria.OrEmpty())
            {
                if (keywordMap.ContainsKey(akwid))
                {
                    var bkwid = keywordMap[akwid];
                    toDelete.Add(bkwid);
                    keywordMap.Remove(akwid);
                }
                else
                {
                    Console.Out.WriteLine("missing akwid in keyword map");
                }
            }
            if (toDelete.Any())
            {
                Console.Out.WriteLine("DeleteKeywords ag {0} kw {1}", bagid,
                    string.Join(",", toDelete.Select(i => i.ToString())));
                BingAds.DeleteKeywords(bagid, toDelete);
            }

            if (ag.changedCriteria.OrEmpty().Any())
            {
                var akws =
                    AdWords.GetKeywords(ag.adGroupId, ag.changedCriteria.OrEmpty())
                        .ToDictionaryDuplicateKeyOverride(k => k.Keyword.id, k => k);

                //update | add
                foreach (var akwid in ag.changedCriteria.OrEmpty())
                {
                    var akw = akws.TryGet(akwid);
                    if (akw == null) continue; //already deleted?
                    if (keywordMap.ContainsKey(akwid))
                    {
                        //update
                        toUpdate.Add(KeywordTransform.Update(akw, keywordMap[akwid]));
                    }
                    else
                    {
                        //add
                        toAdd.Add(akw);
                    }
                }

                if (toUpdate.Any())
                {
                    var resp=BingAds.UpdateKeywords(bagid, toUpdate);
                    Console.Out.WriteLine(resp.PartialErrors);
                }

                if (toAdd.Any())
                {
                    var bkws = toAdd.Select(KeywordTransform.Add).ToArray();
                    var added = BingAds.AddKeywords(bagid, bkws);//check all the partial error!
                    for (int i = 0; i < added.KeywordIds.Count; i++)
                    {
                        if (!added.KeywordIds[i].HasValue)
                        {
                            Console.Out.WriteLine("added keyword has no id?");
                            continue;
                        }
                        keywordMap[toAdd[i].Keyword.id] = added.KeywordIds[i].Value;
                    }
                }
            }
            Console.Out.WriteLine("keyword sync: {0} delete {1} update {2} add", toDelete.Count, toUpdate.Count, toAdd.Count);
            //save keyword map
            if (toDelete.Any() || toAdd.Any())
                keywordMap.Write(KeywordMap.GetPath(c.campaignId, ag.adGroupId));
        }
    }
}
