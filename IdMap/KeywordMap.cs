using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g2b.IdMap
{
    using System.IO;
    using Common;
    using Google.Api.Ads.AdWords.v201509;
    using Microsoft.BingAds.CampaignManagement;

    public class KeywordMap:MapBase
    {
        private const string Fp = @"keyword.map";
        private const string StatFp = @"keyword.stat";

        public static string GetPath(long acid, long aagid)
        {
            return Path.Combine(Root, acid.ToString(), aagid.ToString(), Fp);
        }

        public static void Build(long acid, long aagid, long bcid, long bagid)
        {
            var root = Path.Combine(Root, acid.ToString(), aagid.ToString());
            Console.Out.WriteLine("touch {0}", root);
            if (!Directory.Exists(root)) Directory.CreateDirectory(root);

            Console.Out.WriteLine("build keyword map for campaign/adgroup {0}/{1}=>{2}/{3}", acid, aagid, bcid, bagid);
            var akws = AdWords.GetKeywords(new[] { aagid }).OrEmpty().Where(a => a.Ac.userStatus != UserStatus.REMOVED).ToList();
            Console.Out.WriteLine("adwords keywords count = {0} for ad group {1}", akws.Count, aagid);
            var bkws =
                BingAds.GetKeywords(new[] {bagid})
                    .Where(a => a.Id.HasValue && a.MatchType.HasValue && a.MatchType != MatchType.Content)
                    .ToList();
            Console.Out.WriteLine("bingads keywords count = {0} for ad group {1}", bkws.Count, bagid);

            var bmapped = new HashSet<long>();
            var bindex =
                bkws.ToDictionaryDuplicateKeyOverride(
                    c => Tuple.Create(c.Text.ToLowerInvariant(), Transform.KeywordTransform.MapMatchType(c.MatchType.Value)), c => c);
            var map = new Dictionary<long, long>();
            foreach (var ac in akws)
            {
                var key = Tuple.Create(ac.Keyword.text.ToLowerInvariant(), ac.Keyword.matchType);
                var bc = bindex.TryGet(key);
                if (bc == null) continue;
                var bid = bc.Id.Value;
                if (bmapped.Contains(bid)) continue;
                map[ac.Keyword.id] = bid;
                bmapped.Add(bid);
            }
            var gonly = akws.Where(c => !map.ContainsKey(c.Keyword.id)).ToList();
            var bonly = bkws.Where(c => !bmapped.Contains(c.Id.Value)).ToList();
            using (var o = new StreamWriter(Path.Combine(root, StatFp)))
            {
                var stat = string.Format("gonly:{0} bonly:{1} map:{2}", gonly.Count, bonly.Count, map.Count);
                o.WriteLine(stat);
                Console.Out.WriteLine(stat);
                foreach (var c in gonly)
                {
                    o.WriteLine("gonly keyword {0} {1}", c.Keyword.text, c.Keyword.matchType);
                }
                foreach (var c in bonly)
                {
                    o.WriteLine("bonly keyword {0} {1}", c.Text, c.MatchType);
                }
            }
            map.Write(Path.Combine(root, Fp));

        }
    }
}
