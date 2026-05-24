using System;

namespace Project147.GameCore.Level
{
    public readonly struct WaveSpawnResult : IEquatable<WaveSpawnResult>
    {
        public WaveSpawnResult(WaveSpawnState state, int spawnCount)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (spawnCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(spawnCount), "Spawn count cannot be negative.");
            }

            State = state;
            SpawnCount = spawnCount;
        }

        public WaveSpawnState State { get; }

        public int SpawnCount { get; }

        public bool Equals(WaveSpawnResult other)
        {
            return Equals(State, other.State) && SpawnCount == other.SpawnCount;
        }

        public override bool Equals(object obj)
        {
            return obj is WaveSpawnResult other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(State, SpawnCount);
        }
    }
}
