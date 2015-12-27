using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g2b.Transform
{
    using Common;
    using Google.Api.Ads.AdWords.v201509;
    public class Listing
    {
        public Keyword Keyword
        {
            get { return Ac.criterion as Keyword; }
        }

        public BiddableAdGroupCriterion Ac;

        public double? Bid
        {
            get
            {
                var bids = Ac.biddingStrategyConfiguration.bids;
                if (bids.OrEmpty().Any())
                {
                    var bid = bids[0] as CpcBid;
                    if (bid != null)
                    {
                        return bid.bid.microAmount*1.0/1000000;
                    }
                }
                return null;
            }
        }
    }
    public class KeywordTransform
    {
        public static KeywordMatchType MapMatchType(Microsoft.BingAds.CampaignManagement.MatchType mt)
        {
            switch (mt)
            {
                case Microsoft.BingAds.CampaignManagement.MatchType.Exact:
                    return KeywordMatchType.EXACT;
                case Microsoft.BingAds.CampaignManagement.MatchType.Phrase:
                    return KeywordMatchType.PHRASE;
                case Microsoft.BingAds.CampaignManagement.MatchType.Broad:
                    return KeywordMatchType.BROAD;
            }
            throw new Exception(mt.ToString());
        }

        public static Microsoft.BingAds.CampaignManagement.MatchType MapMatchType(KeywordMatchType mt)
        {
            switch (mt)
            {
                case KeywordMatchType.EXACT:
                    return Microsoft.BingAds.CampaignManagement.MatchType.Exact;
                case KeywordMatchType.PHRASE:
                    return Microsoft.BingAds.CampaignManagement.MatchType.Phrase;
                case KeywordMatchType.BROAD:
                    return Microsoft.BingAds.CampaignManagement.MatchType.Broad;
            }
            throw new Exception(mt.ToString());
        }

        public static Microsoft.BingAds.CampaignManagement.KeywordStatus MapStatus(UserStatus s)
        {
            switch (s)
            {
                case UserStatus.ENABLED:
                    return Microsoft.BingAds.CampaignManagement.KeywordStatus.Active;
                case UserStatus.PAUSED:
                    return Microsoft.BingAds.CampaignManagement.KeywordStatus.Paused;
                case UserStatus.REMOVED:
                    return Microsoft.BingAds.CampaignManagement.KeywordStatus.Deleted;
            }
            throw new Exception(s.ToString());
        }

        public static Microsoft.BingAds.CampaignManagement.Keyword Add(Listing k)
        {
            return new Microsoft.BingAds.CampaignManagement.Keyword
            {
                Text = k.Keyword.text,
                MatchType = MapMatchType(k.Keyword.matchType),
                Bid = new Microsoft.BingAds.CampaignManagement.Bid {Amount = k.Bid},
                Status = MapStatus(k.Ac.userStatus),
            };
        }

        public static Microsoft.BingAds.CampaignManagement.Keyword Update(Listing k, long kwid)
        {
            return new Microsoft.BingAds.CampaignManagement.Keyword
            {
                Id = kwid,
                Bid = new Microsoft.BingAds.CampaignManagement.Bid { Amount = k.Bid },
                Status = MapStatus(k.Ac.userStatus),
            };
        }
    }
}
