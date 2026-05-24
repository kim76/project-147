using System;

namespace Project147.GameCore.Level
{
    public sealed class WaveDefinition
    {
        public WaveDefinition(int alienCount, float secondsBetweenSpawns, int clearReward)
        {
            if (alienCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(alienCount), "Wave alien count must be greater than zero.");
            }

            if (secondsBetweenSpawns <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(secondsBetweenSpawns),
                    "Seconds between spawns must be greater than zero.");
            }

            if (clearReward < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(clearReward), "Wave clear reward cannot be negative.");
            }

            AlienCount = alienCount;
            SecondsBetweenSpawns = secondsBetweenSpawns;
            ClearReward = clearReward;
        }

        public int AlienCount { get; }

        public float SecondsBetweenSpawns { get; }

        public int ClearReward { get; }
    }
}
