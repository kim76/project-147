using System;
using System.Collections.Generic;

namespace Project147.PlatformServices.Monetisation
{
    public sealed class RewardedAdOfferTracker
    {
        private readonly Dictionary<string, int> offerCounts = new Dictionary<string, int>();

        public bool CanOffer(RewardedAdOpportunityDefinition opportunity)
        {
            if (opportunity == null)
            {
                throw new ArgumentNullException(nameof(opportunity));
            }

            return GetOfferCount(opportunity.Id) < opportunity.MaxOffersPerRun;
        }

        public RewardedAdOfferTracker RecordOffer(RewardedAdOpportunityDefinition opportunity)
        {
            if (opportunity == null)
            {
                throw new ArgumentNullException(nameof(opportunity));
            }

            if (!CanOffer(opportunity))
            {
                throw new InvalidOperationException($"Rewarded ad opportunity '{opportunity.Id}' has reached its run limit.");
            }

            var counts = new Dictionary<string, int>(offerCounts);
            counts[opportunity.Id] = GetOfferCount(opportunity.Id) + 1;
            return new RewardedAdOfferTracker(counts);
        }

        public int GetOfferCount(string opportunityId)
        {
            if (string.IsNullOrWhiteSpace(opportunityId))
            {
                throw new ArgumentException("Rewarded ad opportunity id is required.", nameof(opportunityId));
            }

            return offerCounts.TryGetValue(opportunityId, out var count) ? count : 0;
        }

        public int TotalOffers
        {
            get
            {
                var total = 0;

                foreach (var count in offerCounts.Values)
                {
                    total += count;
                }

                return total;
            }
        }

        private RewardedAdOfferTracker(Dictionary<string, int> offerCounts)
        {
            this.offerCounts = offerCounts;
        }

        public RewardedAdOfferTracker()
        {
        }
    }
}
