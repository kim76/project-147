using System;

namespace Project147.GameCore.Level
{
    public readonly struct WaveIntelEntry : IEquatable<WaveIntelEntry>
    {
        public WaveIntelEntry(string alienId, int count)
        {
            if (string.IsNullOrWhiteSpace(alienId))
            {
                throw new ArgumentException("Alien id is required.", nameof(alienId));
            }

            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Intel count must be greater than zero.");
            }

            AlienId = alienId;
            Count = count;
        }

        public string AlienId { get; }

        public int Count { get; }

        public bool Equals(WaveIntelEntry other)
        {
            return AlienId == other.AlienId && Count == other.Count;
        }

        public override bool Equals(object obj)
        {
            return obj is WaveIntelEntry other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AlienId, Count);
        }
    }
}
