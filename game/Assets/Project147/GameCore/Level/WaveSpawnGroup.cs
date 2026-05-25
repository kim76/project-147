using System;

namespace Project147.GameCore.Level
{
    public readonly struct WaveSpawnGroup : IEquatable<WaveSpawnGroup>
    {
        public WaveSpawnGroup(string alienId, int count)
        {
            if (string.IsNullOrWhiteSpace(alienId))
            {
                throw new ArgumentException("Alien id is required.", nameof(alienId));
            }

            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Spawn group count must be greater than zero.");
            }

            AlienId = alienId;
            Count = count;
        }

        public string AlienId { get; }

        public int Count { get; }

        public bool Equals(WaveSpawnGroup other)
        {
            return AlienId == other.AlienId && Count == other.Count;
        }

        public override bool Equals(object obj)
        {
            return obj is WaveSpawnGroup other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AlienId, Count);
        }
    }
}
