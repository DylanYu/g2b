using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g2b.IdMap
{
    using System.IO;
    using System.Threading;
    using Common;
    using Google.Api.Ads.AdWords.v201509;


    public class MapBase
    {
        protected const string Root = "data";
    }
    public class CampaignMap : MapBase
    {
        private const string Fp = @"campaign.map";
        private const string StatFp = @"campaign.stat";
        public static IDictionary<long, long> Build()
        {
            Console.Out.WriteLine("purge root");
            if (Directory.Exists(Root))
            {
                Directory.Delete(Root, true);
            }
            Directory.CreateDirectory(Root);
            Thread.Sleep(TimeSpan.FromSeconds(1));
            Console.Out.WriteLine("build campaign map");
            var acs = AdWords.GetCampaigns().OrEmpty().Where(c=>c.status!=CampaignStatus.REMOVED).ToList();//assume no duplicate id
            Console.Out.WriteLine("get adwords campaigns {0}", acs.Count());
            var bcs = BingAds.GetCampaigns().Where(c=>c.Id.HasValue).ToList();
            Console.Out.WriteLine("get bingads campaigns {0}", bcs.Count);
            var bmapped = new HashSet<long>();
            var bindex = bcs.ToDictionaryDuplicateKeyOverride(c => c.Name.ToLowerInvariant(), c => c);
            var map = new Dictionary<long, long>();
            foreach (var ac in acs)
            {
                var bc = bindex.TryGet(ac.name.ToLowerInvariant());
                if (bc == null) continue;
                var bid = bc.Id.Value;
                if(bmapped.Contains(bid))continue;
                map[ac.id] = bid;
                bmapped.Add(bid);
            }
            var gonly = acs.Where(c => !map.ContainsKey(c.id)).ToList();
            var bonly = bcs.Where(c => !bmapped.Contains(c.Id.Value)).ToList();
            using (var o = new StreamWriter(Path.Combine(Root, StatFp)))
            {
                var stat = string.Format("gonly:{0} bonly:{1} map:{2}", gonly.Count, bonly.Count, map.Count);
                o.WriteLine(stat);
                Console.Out.WriteLine(stat);
                foreach (var c in gonly)
                {
                    o.WriteLine("gonly campaign {0}", c.name);
                }
                foreach (var c in bonly)
                {
                    o.WriteLine("bonly campaign {0}", c.Name);
                }
            }
            map.Write(Path.Combine(Root, Fp));
            return map;
        }
    }
}
