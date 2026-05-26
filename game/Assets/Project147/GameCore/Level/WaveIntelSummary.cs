using System;
using System.Collections.Generic;

namespace Project147.GameCore.Level
{
    public sealed class WaveIntelSummary
    {
        public WaveIntelSummary(
            int waveNumber,
            IReadOnlyList<WaveIntelEntry> entries,
            IReadOnlyList<string> tags,
            int clearReward,
            int threatRating = 1)
        {
            if (waveNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(waveNumber), "Wave number must be greater than zero.");
            }

            if (entries == null)
            {
                throw new ArgumentNullException(nameof(entries));
            }

            if (tags == null)
            {
                throw new ArgumentNullException(nameof(tags));
            }

            if (entries.Count == 0)
            {
                throw new ArgumentException("Wave intel requires at least one entry.", nameof(entries));
            }

            if (clearReward < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(clearReward), "Clear reward cannot be negative.");
            }

            if (threatRating < 1 || threatRating > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(threatRating), "Threat rating must be between one and five.");
            }

            var totalAliens = 0;

            foreach (var entry in entries)
            {
                if (string.IsNullOrWhiteSpace(entry.AlienId))
                {
                    throw new ArgumentException("Wave intel entries require alien ids.", nameof(entries));
                }

                if (entry.Count <= 0)
                {
                    throw new ArgumentException("Wave intel entries require positive counts.", nameof(entries));
                }

                totalAliens += entry.Count;
            }

            foreach (var tag in tags)
            {
                if (string.IsNullOrWhiteSpace(tag))
                {
                    throw new ArgumentException("Wave intel tags cannot be empty.", nameof(tags));
                }
            }

            WaveNumber = waveNumber;
            Entries = new List<WaveIntelEntry>(entries);
            Tags = new List<string>(tags);
            TotalAliens = totalAliens;
            ClearReward = clearReward;
            ThreatRating = threatRating;
        }

        public int WaveNumber { get; }

        public int TotalAliens { get; }

        public int ClearReward { get; }

        public int ThreatRating { get; }

        public IReadOnlyList<WaveIntelEntry> Entries { get; }

        public IReadOnlyList<string> Tags { get; }
    }
}
