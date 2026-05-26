using System;

namespace Project147.GameCore.Level
{
    public sealed class LevelProgressSnapshot
    {
        public LevelProgressSnapshot(
            string levelId,
            int runsCompleted,
            int victories,
            int bestStars,
            int bestWavesCleared,
            int bestPerfectWaves)
        {
            if (string.IsNullOrWhiteSpace(levelId))
            {
                throw new ArgumentException("Level id is required.", nameof(levelId));
            }

            LevelId = levelId;
            RunsCompleted = runsCompleted;
            Victories = victories;
            BestStars = bestStars;
            BestWavesCleared = bestWavesCleared;
            BestPerfectWaves = bestPerfectWaves;
        }

        public string LevelId { get; }

        public int RunsCompleted { get; }

        public int Victories { get; }

        public int BestStars { get; }

        public int BestWavesCleared { get; }

        public int BestPerfectWaves { get; }

        public LevelProgressState Restore()
        {
            return new LevelProgressState(
                RunsCompleted,
                Victories,
                BestStars,
                BestWavesCleared,
                BestPerfectWaves);
        }
    }
}
