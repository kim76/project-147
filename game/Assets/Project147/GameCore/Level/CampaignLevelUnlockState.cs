using System;
using System.Collections.Generic;

namespace Project147.GameCore.Level
{
    public sealed class CampaignLevelUnlockState
    {
        private readonly HashSet<string> unlockedLevelIds;

        public CampaignLevelUnlockState(
            IReadOnlyList<LevelUnlockRule> rules,
            CampaignProgressState campaign)
        {
            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            if (campaign == null)
            {
                throw new ArgumentNullException(nameof(campaign));
            }

            var ids = new HashSet<string>();
            var unlocked = new HashSet<string>();
            var ordered = new List<LevelUnlockRule>();

            foreach (var rule in rules)
            {
                if (rule == null)
                {
                    throw new ArgumentNullException(nameof(rules), "Unlock rules cannot contain null values.");
                }

                if (!ids.Add(rule.LevelId))
                {
                    throw new ArgumentException("Unlock rules cannot contain duplicate level ids.", nameof(rules));
                }

                ordered.Add(rule);

                if (rule.IsUnlocked(campaign))
                {
                    unlocked.Add(rule.LevelId);
                }
            }

            if (ordered.Count == 0)
            {
                throw new ArgumentException("At least one unlock rule is required.", nameof(rules));
            }

            Rules = ordered;
            unlockedLevelIds = unlocked;
            UnlockedLevelIds = unlocked;
        }

        public IReadOnlyList<LevelUnlockRule> Rules { get; }

        public IReadOnlyCollection<string> UnlockedLevelIds { get; }

        public bool IsUnlocked(string levelId)
        {
            if (string.IsNullOrWhiteSpace(levelId))
            {
                throw new ArgumentException("Level id is required.", nameof(levelId));
            }

            return unlockedLevelIds.Contains(levelId);
        }
    }
}
