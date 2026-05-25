using System;

namespace Project147.GameCore.Level
{
    public readonly struct WaveSpawnEntry : IEquatable<WaveSpawnEntry>
    {
        public WaveSpawnEntry(string alienId)
        {
            if (string.IsNullOrWhiteSpace(alienId))
            {
                throw new ArgumentException("Alien id is required.", nameof(alienId));
            }

            AlienId = alienId;
        }

        public string AlienId { get; }

        public bool Equals(WaveSpawnEntry other)
        {
            return AlienId == other.AlienId;
        }

        public override bool Equals(object obj)
        {
            return obj is WaveSpawnEntry other && Equals(other);
        }

        public override int GetHashCode()
        {
            return AlienId == null ? 0 : AlienId.GetHashCode();
        }
    }
}
