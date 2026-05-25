using System;
using System.Collections.Generic;

namespace Project147.GameCore.Level
{
    public readonly struct WaveSpawnResult : IEquatable<WaveSpawnResult>
    {
        public WaveSpawnResult(WaveSpawnState state, int spawnCount)
            : this(state, CreateLegacySpawnEntries(spawnCount))
        {
        }

        public WaveSpawnResult(WaveSpawnState state, IReadOnlyList<WaveSpawnEntry> spawnEntries)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (spawnEntries == null)
            {
                throw new ArgumentNullException(nameof(spawnEntries));
            }

            State = state;
            SpawnEntries = new List<WaveSpawnEntry>(spawnEntries);
            SpawnCount = SpawnEntries.Count;
        }

        public WaveSpawnState State { get; }

        public int SpawnCount { get; }

        public IReadOnlyList<WaveSpawnEntry> SpawnEntries { get; }

        public bool Equals(WaveSpawnResult other)
        {
            if (!Equals(State, other.State) || SpawnCount != other.SpawnCount)
            {
                return false;
            }

            for (var index = 0; index < SpawnEntries.Count; index++)
            {
                if (!SpawnEntries[index].Equals(other.SpawnEntries[index]))
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is WaveSpawnResult other && Equals(other);
        }

        public override int GetHashCode()
        {
            var hashCode = HashCode.Combine(State, SpawnCount);

            foreach (var entry in SpawnEntries)
            {
                hashCode = HashCode.Combine(hashCode, entry);
            }

            return hashCode;
        }

        private static IReadOnlyList<WaveSpawnEntry> CreateLegacySpawnEntries(int spawnCount)
        {
            if (spawnCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(spawnCount), "Spawn count cannot be negative.");
            }

            var entries = new List<WaveSpawnEntry>();

            for (var index = 0; index < spawnCount; index++)
            {
                entries.Add(new WaveSpawnEntry("default"));
            }

            return entries;
        }
    }
}
