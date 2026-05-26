using System.Collections.Generic;
using System.Linq;

namespace Project147.PlatformServices.Monetisation
{
    public sealed class Project147RewardedAdCatalog
    {
        public IReadOnlyList<RewardedAdOpportunityDefinition> CreateOpportunities()
        {
            return new[]
            {
                new RewardedAdOpportunityDefinition(
                    "double-wave-clear-scrap",
                    RewardedAdOpportunityTrigger.AfterWaveClear,
                    "Double the scrap reward from the cleared wave.",
                    2),
                new RewardedAdOpportunityDefinition(
                    "emergency-base-repair",
                    RewardedAdOpportunityTrigger.AfterDefeat,
                    "Repair the base once and replay the current wave.",
                    1),
                new RewardedAdOpportunityDefinition(
                    "pre-wave-supply-drop",
                    RewardedAdOpportunityTrigger.BeforeWaveStart,
                    "Add bonus scrap before starting the next wave.",
                    1)
            };
        }

        public RewardedAdOpportunityDefinition GetRequiredOpportunity(string id)
        {
            return CreateOpportunities().Single(opportunity => opportunity.Id == id);
        }
    }
}
