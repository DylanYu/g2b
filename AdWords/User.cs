using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g2b
{
    using System.IO;
    using Google.Api.Ads.AdWords.Lib;
    using Google.Api.Ads.AdWords.v201509;

    partial class AdWords
    {
        private static readonly Lazy<AdWordsUser> User=new Lazy<AdWordsUser>(() =>
        {
            var user = new AdWordsUser();
            AdWordsAppConfig config = (AdWordsAppConfig)user.Config;
            config.DeveloperToken = File.ReadAllText(@"d:\devtoken");
            return user;
        });
    }
}
