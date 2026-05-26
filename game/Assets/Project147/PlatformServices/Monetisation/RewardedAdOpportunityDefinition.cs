using System;

namespace Project147.PlatformServices.Monetisation
{
    public sealed class RewardedAdOpportunityDefinition
    {
        public RewardedAdOpportunityDefinition(
            string id,
            RewardedAdOpportunityTrigger trigger,
            string rewardDescription,
            int maxOffersPerRun)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Rewarded ad opportunity id is required.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(rewardDescription))
            {
                throw new ArgumentException("Rewarded ad reward description is required.", nameof(rewardDescription));
            }

            if (maxOffersPerRun <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxOffersPerRun),
                    "Rewarded ad max offers per run must be greater than zero.");
            }

            Id = id;
            Trigger = trigger;
            RewardDescription = rewardDescription;
            MaxOffersPerRun = maxOffersPerRun;
        }

        public string Id { get; }

        public RewardedAdOpportunityTrigger Trigger { get; }

        public string RewardDescription { get; }

        public int MaxOffersPerRun { get; }
    }
}
