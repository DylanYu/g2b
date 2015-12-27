using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g2b
{
    using Common;
    using Google.Api.Ads.AdWords.Lib;
    using Google.Api.Ads.AdWords.v201509;
    using Transform;

    partial class AdWords
    {

        private static readonly Lazy<AdGroupCriterionService> KeywordService =
            new Lazy<AdGroupCriterionService>(
                () => (AdGroupCriterionService)User.Value.GetService(AdWordsService.v201509.AdGroupCriterionService));

        //ad group id lost in Keyword
        public static IEnumerable<Listing> GetKeywords(IEnumerable<long> agids)
        {
            var str = agids.Select(i => i.ToString()).ToArray();
            var sel0 = new Selector
            {
                fields = new[] {"Id", "KeywordText", "KeywordMatchType", "CpcBid", "Status", "AdGroupId"},
                predicates =
                    new[]
                    {
                        new Predicate
                        {
                            field = "AdGroupId",
                            @operator = PredicateOperator.IN,
                            operatorSpecified = true,
                            values = str
                        }
                    }
            };
            return
                from c in KeywordService.Value.get(sel0).entries.OrEmpty()
                let ac = c as BiddableAdGroupCriterion
                where ac != null
                let k = c.criterion as Keyword
                where k != null
                select new Listing {Ac = ac};
        }

        public static IEnumerable<Listing> GetKeywords(long agid, IEnumerable<long> kwids)
        {
            var str = kwids.Select(i => i.ToString()).ToArray();
            var sel0 = new Selector
            {
                fields = new[] { "Id", "KeywordText", "KeywordMatchType", "CpcBid", "Status", "AdGroupId" },
                predicates =
                    new[]
                    {
                        new Predicate
                        {
                            field = "Id",
                            @operator = PredicateOperator.IN,
                            operatorSpecified = true,
                            values = str
                        }
                    }
            };
            return
                from c in KeywordService.Value.get(sel0).entries.OrEmpty()
                let ac = c as BiddableAdGroupCriterion
                where ac != null
                let k = c.criterion as Keyword
                where k != null
                select new Listing { Ac = ac };
        }

        public static AdGroupCriterionReturnValue SetKeywords(IEnumerable<Listing> cs, bool isDelete = false)
        {
            return KeywordService.Value.mutate(
                cs.Select(
                    c =>
                        new AdGroupCriterionOperation
                        {
                            operand = c.Ac,
                            @operator = isDelete ? Operator.REMOVE : Operator.SET
                        }).ToArray());
        }

        public static AdGroupCriterionReturnValue AddKeywords(IEnumerable<BiddableAdGroupCriterion> cs)
        {
            return KeywordService.Value.mutate(
                cs.Select(c => new AdGroupCriterionOperation
                {
                    operand = c,
                    @operator = Operator.ADD
                }).ToArray());
        }

    }
}
