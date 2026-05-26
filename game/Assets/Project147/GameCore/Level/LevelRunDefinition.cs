using System;

namespace Project147.GameCore.Level
{
    public sealed class LevelRunDefinition
    {
        public LevelRunDefinition(
            LevelLayoutDefinition layout,
            int startingCurrency,
            int baseHealth,
            int totalWaves,
            int perfectWaveScrapBonus)
        {
            if (layout == null)
            {
                throw new ArgumentNullException(nameof(layout));
            }

            if (startingCurrency < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startingCurrency), "Starting currency cannot be negative.");
            }

            if (baseHealth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(baseHealth), "Base health must be greater than zero.");
            }

            if (totalWaves <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(totalWaves), "Total waves must be greater than zero.");
            }

            if (perfectWaveScrapBonus < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(perfectWaveScrapBonus), "Perfect wave bonus cannot be negative.");
            }

            Layout = layout;
            StartingCurrency = startingCurrency;
            BaseHealth = baseHealth;
            TotalWaves = totalWaves;
            PerfectWaveScrapBonus = perfectWaveScrapBonus;
        }

        public LevelLayoutDefinition Layout { get; }

        public int StartingCurrency { get; }

        public int BaseHealth { get; }

        public int TotalWaves { get; }

        public int PerfectWaveScrapBonus { get; }
    }
}
