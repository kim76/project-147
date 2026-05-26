using System;
using System.Collections.Generic;

namespace Project147.GameCore.Level
{
    public sealed class CampaignProgressState
    {
        private readonly IReadOnlyDictionary<string, LevelProgressState> levelProgressById;

        public CampaignProgressState()
            : this(new Dictionary<string, LevelProgressState>())
        {
        }

        public CampaignProgressState(IReadOnlyDictionary<string, LevelProgressState> levelProgressById)
        {
            if (levelProgressById == null)
            {
                throw new ArgumentNullException(nameof(levelProgressById));
            }

            foreach (var entry in levelProgressById)
            {
                if (string.IsNullOrWhiteSpace(entry.Key))
                {
                    throw new ArgumentException("Level id is required.", nameof(levelProgressById));
                }

                if (entry.Value == null)
                {
                    throw new ArgumentNullException(nameof(levelProgressById), "Level progress cannot contain null values.");
                }
            }

            this.levelProgressById = new Dictionary<string, LevelProgressState>(levelProgressById);
        }

        public int TotalStars
        {
            get
            {
                var total = 0;

                foreach (var progress in levelProgressById.Values)
                {
                    total += progress.BestStars;
                }

                return total;
            }
        }

        public IReadOnlyDictionary<string, LevelProgressState> LevelProgressById
        {
            get { return new Dictionary<string, LevelProgressState>(levelProgressById); }
        }

        public LevelProgressState GetProgress(string levelId)
        {
            if (string.IsNullOrWhiteSpace(levelId))
            {
                throw new ArgumentException("Level id is required.", nameof(levelId));
            }

            return levelProgressById.TryGetValue(levelId, out var progress)
                ? progress
                : new LevelProgressState();
        }

        public CampaignProgressApplicationResult ApplyRunSummary(string levelId, RunSummaryState summary)
        {
            if (string.IsNullOrWhiteSpace(levelId))
            {
                throw new ArgumentException("Level id is required.", nameof(levelId));
            }

            var levelResult = GetProgress(levelId).ApplyRunSummary(summary);
            var progress = new Dictionary<string, LevelProgressState>(levelProgressById)
            {
                [levelId] = levelResult.State
            };

            return new CampaignProgressApplicationResult(
                new CampaignProgressState(progress),
                levelResult);
        }
    }
}
