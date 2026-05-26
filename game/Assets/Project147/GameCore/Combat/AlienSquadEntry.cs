using System;

namespace Project147.GameCore.Combat
{
    public readonly struct AlienSquadEntry : IEquatable<AlienSquadEntry>
    {
        public AlienSquadEntry(string alienId, int count, int costPerAlien)
        {
            if (string.IsNullOrWhiteSpace(alienId))
            {
                throw new ArgumentException("Alien id is required.", nameof(alienId));
            }

            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Alien count must be greater than zero.");
            }

            if (costPerAlien < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(costPerAlien), "Alien squad cost cannot be negative.");
            }

            AlienId = alienId;
            Count = count;
            CostPerAlien = costPerAlien;
        }

        public string AlienId { get; }

        public int Count { get; }

        public int CostPerAlien { get; }

        public int TotalCost
        {
            get { return Count * CostPerAlien; }
        }

        public bool Equals(AlienSquadEntry other)
        {
            return AlienId == other.AlienId
                && Count == other.Count
                && CostPerAlien == other.CostPerAlien;
        }

        public override bool Equals(object obj)
        {
            return obj is AlienSquadEntry other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AlienId, Count, CostPerAlien);
        }
    }
}
