using System;
using System.Collections.Generic;

namespace Project147.GameCore.Level
{
    public sealed class CampaignProgressSnapshot
    {
        public CampaignProgressSnapshot(IReadOnlyList<LevelProgressSnapshot> levels)
        {
            if (levels == null)
            {
                throw new ArgumentNullException(nameof(levels));
            }

            var snapshots = new List<LevelProgressSnapshot>();
            var ids = new HashSet<string>();

            foreach (var level in levels)
            {
                if (level == null)
                {
                    throw new ArgumentNullException(nameof(levels), "Campaign progress snapshot cannot contain null levels.");
                }

                if (!ids.Add(level.LevelId))
                {
                    throw new ArgumentException("Campaign progress snapshot cannot contain duplicate levels.", nameof(levels));
                }

                snapshots.Add(level);
            }

            Levels = snapshots;
        }

        public IReadOnlyList<LevelProgressSnapshot> Levels { get; }

        public static CampaignProgressSnapshot Capture(CampaignProgressState campaign)
        {
            if (campaign == null)
            {
                throw new ArgumentNullException(nameof(campaign));
            }

            var levels = new List<LevelProgressSnapshot>();

            foreach (var entry in campaign.LevelProgressById)
            {
                levels.Add(new LevelProgressSnapshot(
                    entry.Key,
                    entry.Value.RunsCompleted,
                    entry.Value.Victories,
                    entry.Value.BestStars,
                    entry.Value.BestWavesCleared,
                    entry.Value.BestPerfectWaves));
            }

            return new CampaignProgressSnapshot(levels);
        }

        public CampaignProgressState Restore()
        {
            var progress = new Dictionary<string, LevelProgressState>();

            foreach (var level in Levels)
            {
                progress[level.LevelId] = level.Restore();
            }

            return new CampaignProgressState(progress);
        }
    }
}
