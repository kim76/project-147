using System;

namespace Project147.GameCore.Level
{
    public readonly struct RewardResult : IEquatable<RewardResult>
    {
        public RewardResult(RewardSource source, int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Reward amount cannot be negative.");
            }

            Source = source;
            Amount = amount;
        }

        public RewardSource Source { get; }

        public int Amount { get; }

        public bool Equals(RewardResult other)
        {
            return Source == other.Source && Amount == other.Amount;
        }

        public override bool Equals(object obj)
        {
            return obj is RewardResult other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Source, Amount);
        }
    }
}
