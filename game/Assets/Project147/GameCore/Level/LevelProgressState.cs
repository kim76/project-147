using System;

namespace Project147.GameCore.Level
{
    public sealed class LevelProgressState
    {
        public LevelProgressState()
            : this(0, 0, 0, 0, 0)
        {
        }

        private LevelProgressState(
            int runsCompleted,
            int victories,
            int bestStars,
            int bestWavesCleared,
            int bestPerfectWaves)
        {
            if (runsCompleted < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(runsCompleted), "Run count cannot be negative.");
            }

            if (victories < 0 || victories > runsCompleted)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(victories),
                    "Victory count must be between zero and completed runs.");
            }

            if (bestStars < 0 || bestStars > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(bestStars), "Best stars must be between zero and three.");
            }

            if (bestWavesCleared < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bestWavesCleared), "Best waves cleared cannot be negative.");
            }

            if (bestPerfectWaves < 0 || bestPerfectWaves > bestWavesCleared)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(bestPerfectWaves),
                    "Best perfect waves must be between zero and best waves cleared.");
            }

            RunsCompleted = runsCompleted;
            Victories = victories;
            BestStars = bestStars;
            BestWavesCleared = bestWavesCleared;
            BestPerfectWaves = bestPerfectWaves;
        }

        public int RunsCompleted { get; }

        public int Victories { get; }

        public int BestStars { get; }

        public int BestWavesCleared { get; }

        public int BestPerfectWaves { get; }

        public bool HasThreeStarClear
        {
            get { return BestStars == 3; }
        }

        public LevelProgressApplicationResult ApplyRunSummary(RunSummaryState summary)
        {
            if (summary == null)
            {
                throw new ArgumentNullException(nameof(summary));
            }

            if (summary.Outcome == RunOutcome.InProgress)
            {
                throw new ArgumentException("Only completed runs can update level progress.", nameof(summary));
            }

            var previousBestStars = BestStars;
            var previousBestWaves = BestWavesCleared;
            var previousBestPerfectWaves = BestPerfectWaves;
            var nextBestStars = Math.Max(BestStars, summary.StarRating);
            var nextBestWaves = Math.Max(BestWavesCleared, summary.WavesCleared);
            var nextBestPerfectWaves = Math.Max(BestPerfectWaves, summary.PerfectWaves);
            var nextState = new LevelProgressState(
                RunsCompleted + 1,
                summary.Outcome == RunOutcome.Victory ? Victories + 1 : Victories,
                nextBestStars,
                nextBestWaves,
                nextBestPerfectWaves);

            return new LevelProgressApplicationResult(
                nextState,
                nextBestStars > previousBestStars,
                nextBestWaves > previousBestWaves,
                nextBestPerfectWaves > previousBestPerfectWaves);
        }
    }
}
