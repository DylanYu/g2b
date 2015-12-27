using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g2b.IdMap
{
    using System.Collections.Concurrent;
    using System.IO;
    using Common;
    using Google.Api.Ads.AdWords.v201509;

    public class AdGroupMap:MapBase
    {
        private const string Fp = @"adgroup.map";
        private const string StatFp = @"adgroup.stat";

        public static string GetPath(long acid)
        {
            return Path.Combine(Root, acid.ToString(), Fp);
        }

        public static IDictionary<long, long> Build(long acid, long bcid)
        {
            var root = Path.Combine(Root, acid.ToString());
            Console.Out.WriteLine("touch {0}", root);
            if (!Directory.Exists(root)) Directory.CreateDirectory(root);

            Console.Out.WriteLine("build ad group map for campaign {0}=>{1}", acid, bcid);
            var aags = AdWords.GetAdGroups(new [] {acid}).OrEmpty().Where(a=>a.status!=AdGroupStatus.REMOVED).ToList();
            Console.Out.WriteLine("adwords ad groups count = {0} for campaign {1}", aags.Count, acid);
            var bags = BingAds.GetAdGroups(new [] {bcid}).Where(a=>a.Id.HasValue).ToList();
            Console.Out.WriteLine("bingads ad groups count = {0} for campaign {1}", bags.Count, bcid);

            var bmapped = new HashSet<long>();
            var bindex = bags.ToDictionaryDuplicateKeyOverride(c => c.Name.ToLowerInvariant(), c => c);
            var map = new Dictionary<long, long>();
            foreach (var ac in aags)
            {
                var bc = bindex.TryGet(ac.name.ToLowerInvariant());
                if (bc == null) continue;
                var bid = bc.Id.Value;
                if (bmapped.Contains(bid)) continue;
                map[ac.id] = bid;
                bmapped.Add(bid);
            }
            var gonly = aags.Where(c => !map.ContainsKey(c.id)).ToList();
            var bonly = bags.Where(c => !bmapped.Contains(c.Id.Value)).ToList();
            using (var o = new StreamWriter(Path.Combine(root, StatFp)))
            {
                var stat = string.Format("gonly:{0} bonly:{1} map:{2}", gonly.Count, bonly.Count, map.Count);
                o.WriteLine(stat);
                Console.Out.WriteLine(stat);
                foreach (var c in gonly)
                {
                    o.WriteLine("gonly ad group {0}", c.name);
                }
                foreach (var c in bonly)
                {
                    o.WriteLine("bonly ad group {0}", c.Name);
                }
            }
            map.Write(Path.Combine(root, Fp));
            return map;
        } 
    }
}
