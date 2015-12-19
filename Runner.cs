using System;

namespace g2b
{

    class Runner
    {
        public static void Run(string[] args)
        {
            Console.Out.WriteLine("BingAds");
            BingAds.GetCampaigns();

            Console.Out.WriteLine("AdWords");
            AdWords.GetCampaigns();
        }
    }
}
