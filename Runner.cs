using System;

namespace g2b
{
    using RandomAccountGen;

    class Runner
    {
        public static void Run(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Out.WriteLine("support commands: gen | smoke | init | sync");
                return;
            }
            switch (args[0])
            {
                case "gen":
                    Gen();
                    break;
                case "smoke":
                    Smoke();
                    break;
                case "init":
                    Init();
                    break;
                case "sync":
                    Sync();
                    break;
            }
            if (args.Length == 1)
            {
                Console.Out.WriteLine("press any key to exit");
                Console.ReadLine();
            }
        }

        static void Gen()
        {
            var cs=CampaignGen.Gen();
            foreach (var campaign in cs)
            {
                Console.Out.WriteLine("selected campaign {0}", campaign.name);
            }
            var ags=AdGroupGen.Gen(cs);
            foreach (var adGroup in ags)
            {
                Console.Out.WriteLine("selected ad group {0} under {1}", adGroup.name, adGroup.campaignName);
            }
            KeywordGen.Gen(ags);
        }

        static void Smoke()
        {
            Console.Out.WriteLine("smoke test of account settings");
            Console.Out.WriteLine("BingAds");
            var bcs = BingAds.GetCampaigns();
            foreach (var campaign in bcs)
            {
                Console.Out.WriteLine("id:{0},name:{1}", campaign.Id, campaign.Name);
            }

            Console.Out.WriteLine("AdWords");
            var acs = AdWords.GetCampaigns();
            foreach (var campaign in acs)
            {
                Console.Out.WriteLine("id:{0} name:{1}", campaign.id, campaign.name);
            }
        }

        static void Init()
        {
            var cmap = IdMap.CampaignMap.Build();
            foreach (var ckv in cmap)
            {
                var agmap = IdMap.AdGroupMap.Build(ckv.Key, ckv.Value);
                foreach (var agkv in agmap)
                {
                    IdMap.KeywordMap.Build(ckv.Key, agkv.Key, ckv.Value, agkv.Value);
                }
            }
        }

        static void Sync()
        {
            
        }
    }
}
