using System;

namespace Project147.GameCore.Level
{
    public sealed class LevelProgressApplicationResult
    {
        public LevelProgressApplicationResult(
            LevelProgressState state,
            bool improvedStars,
            bool improvedWavesCleared,
            bool improvedPerfectWaves)
        {
            State = state ?? throw new ArgumentNullException(nameof(state));
            ImprovedStars = improvedStars;
            ImprovedWavesCleared = improvedWavesCleared;
            ImprovedPerfectWaves = improvedPerfectWaves;
        }

        public LevelProgressState State { get; }

        public bool ImprovedStars { get; }

        public bool ImprovedWavesCleared { get; }

        public bool ImprovedPerfectWaves { get; }

        public bool HasAnyImprovement
        {
            get { return ImprovedStars || ImprovedWavesCleared || ImprovedPerfectWaves; }
        }
    }
}
