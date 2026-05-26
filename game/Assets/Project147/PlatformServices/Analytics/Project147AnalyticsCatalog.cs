using System.Collections.Generic;
using System.Linq;

namespace Project147.PlatformServices.Analytics
{
    public sealed class Project147AnalyticsCatalog
    {
        public IReadOnlyList<AnalyticsEventDefinition> CreateGameplayEvents()
        {
            return new[]
            {
                new AnalyticsEventDefinition("level_started", new[] { "level_id", "loadout_id" }),
                new AnalyticsEventDefinition("level_completed", new[] { "level_id", "outcome", "stars", "waves_cleared" }),
                new AnalyticsEventDefinition("wave_started", new[] { "level_id", "wave_number", "alien_count" }),
                new AnalyticsEventDefinition("wave_cleared", new[] { "level_id", "wave_number", "perfect_wave" }),
                new AnalyticsEventDefinition("tower_placed", new[] { "level_id", "tower_id", "scrap_cost" }),
                new AnalyticsEventDefinition("tower_upgraded", new[] { "level_id", "tower_id", "upgrade_id", "tower_level" }),
                new AnalyticsEventDefinition("tower_sold", new[] { "level_id", "tower_id", "refund_amount" }),
                new AnalyticsEventDefinition("ability_used", new[] { "level_id", "ability_id", "wave_number" }),
                new AnalyticsEventDefinition("reward_choice_selected", new[] { "level_id", "choice_id", "wave_number" }),
                new AnalyticsEventDefinition("rewarded_ad_offer_shown", new[] { "placement_id", "level_id" })
            };
        }

        public AnalyticsEventDefinition GetRequiredEvent(string name)
        {
            return CreateGameplayEvents().Single(definition => definition.Name == name);
        }
    }
}
