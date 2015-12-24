using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.BingAds;
using Microsoft.BingAds.Bulk;
using Microsoft.BingAds.CampaignManagement;
namespace g2b
{
    class BingAds
    {
        private static long _customerId= 13447361;
        private static long _accountId= 42059793;
        private static string _devToken = @"012QV6D735085967";

        private static readonly Lazy<AuthorizationData> Auth = new Lazy<AuthorizationData>(() =>
        {
            var auth = new OAuthDesktopMobileAuthCodeGrant("0000000040169364");
            var r=auth.RequestAccessAndRefreshTokensAsync(
                "MCSd1XbQd3FqzTwHq1IVpAYqmRi7DtDOi6mDJA!S*gkB9pLKZt7DPP*pacdatvyk79i5vZh8Z8xJhyQwd*SAtRdDBXz6NaXIGLff0qzuP0AM67aVHVvaXWxezZu4Mx5s28R3CCKgLgpCmpjZolYdfO22TS6qA0cfXcLiOLIiVocJcEWoyNgR!xTIbvMzee9HUmtAS0mfX7Ti363tKqUx1ZtCBZLxjs!nQMLFzEFFj4UVhRyeSfVkYyYX5mewb1AZypHvDYa7LoV3LGUb3nwncAGAxHMp0GD3fZIsR94xRijq7Pa2Y5cP1Tc5HgpjXb*P41NRQsPNuRhQHwAnpjozEDeGXq0s4muFJ9riksLU0ilMD");
            r.Wait();
            return new AuthorizationData
            {
                Authentication =auth,
                CustomerId = _customerId,
                AccountId = _accountId,
                DeveloperToken = _devToken
            };
        });

        private static readonly Lazy<ServiceClient<ICampaignManagementService>> CampaignService =
            new Lazy<ServiceClient<ICampaignManagementService>>(
                () => new ServiceClient<ICampaignManagementService>(Auth.Value));

        public static IList<Campaign> GetCampaigns()
        {
            return CampaignService.Value.CallAsync((s, r) => s.GetCampaignsByAccountIdAsync(r),
                new GetCampaignsByAccountIdRequest
                {
                    AccountId = _accountId,
                }).Result.Campaigns;
        }

        public static IEnumerable<AdGroup> GetAdGroups(IEnumerable<long> cids)
        {
            return cids.SelectMany(cid =>
                CampaignService.Value.CallAsync((s, r) => s.GetAdGroupsByCampaignIdAsync(r),
                    new GetAdGroupsByCampaignIdRequest
                    {
                        CampaignId = cid
                    }).Result.AdGroups);
        } 
    }
}
