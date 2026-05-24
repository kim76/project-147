using System;

namespace Project147.GameCore.Combat
{
    public readonly struct DamageRequest : IEquatable<DamageRequest>
    {
        public DamageRequest(float amount, DamageType damageType)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Damage amount cannot be negative.");
            }

            Amount = amount;
            DamageType = damageType;
        }

        public float Amount { get; }

        public DamageType DamageType { get; }

        public bool Equals(DamageRequest other)
        {
            return Amount.Equals(other.Amount) && DamageType == other.DamageType;
        }

        public override bool Equals(object obj)
        {
            return obj is DamageRequest other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Amount, DamageType);
        }
    }
}

