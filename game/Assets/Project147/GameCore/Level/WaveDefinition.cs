using System;
using System.Collections.Generic;

namespace Project147.GameCore.Level
{
    public sealed class WaveDefinition
    {
        private const string DefaultAlienId = "default";

        public WaveDefinition(int alienCount, float secondsBetweenSpawns, int clearReward)
            : this(CreateDefaultComposition(alienCount), secondsBetweenSpawns, clearReward)
        {
        }

        public WaveDefinition(WaveComposition composition, float secondsBetweenSpawns, int clearReward)
        {
            if (composition == null)
            {
                throw new ArgumentNullException(nameof(composition));
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

            Composition = composition;
            SpawnEntries = composition.BuildSpawnEntries();
            AlienCount = SpawnEntries.Count;
            SecondsBetweenSpawns = secondsBetweenSpawns;
            ClearReward = clearReward;
        }

        public int AlienCount { get; }

        public WaveComposition Composition { get; }

        public IReadOnlyList<WaveSpawnEntry> SpawnEntries { get; }

        public float SecondsBetweenSpawns { get; }

        public int ClearReward { get; }

        private static WaveComposition CreateDefaultComposition(int alienCount)
        {
            return new WaveComposition(new[] { new WaveSpawnGroup(DefaultAlienId, alienCount) });
        }
    }
}
