using System;

namespace Project147.GameCore.Level
{
    public sealed class LevelUnlockRule
    {
        public LevelUnlockRule(string levelId, int requiredTotalStars)
        {
            if (string.IsNullOrWhiteSpace(levelId))
            {
                throw new ArgumentException("Level id is required.", nameof(levelId));
            }

            if (requiredTotalStars < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(requiredTotalStars), "Required total stars cannot be negative.");
            }

            LevelId = levelId;
            RequiredTotalStars = requiredTotalStars;
        }

        public string LevelId { get; }

        public int RequiredTotalStars { get; }

        public bool IsUnlocked(CampaignProgressState campaign)
        {
            if (campaign == null)
            {
                throw new ArgumentNullException(nameof(campaign));
            }

            return campaign.TotalStars >= RequiredTotalStars;
        }
    }
}
