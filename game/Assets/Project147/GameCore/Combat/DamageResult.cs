using System;

namespace Project147.GameCore.Combat
{
    public readonly struct DamageResult : IEquatable<DamageResult>
    {
        public DamageResult(float baseAmount, float resistance, float finalAmount)
        {
            if (baseAmount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(baseAmount), "Base amount cannot be negative.");
            }

            if (resistance < 0 || resistance > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(resistance), "Resistance must be between zero and one.");
            }

            if (finalAmount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(finalAmount), "Final amount cannot be negative.");
            }

            BaseAmount = baseAmount;
            Resistance = resistance;
            FinalAmount = finalAmount;
        }

        public float BaseAmount { get; }

        public float Resistance { get; }

        public float FinalAmount { get; }

        public bool Equals(DamageResult other)
        {
            return BaseAmount.Equals(other.BaseAmount)
                && Resistance.Equals(other.Resistance)
                && FinalAmount.Equals(other.FinalAmount);
        }

        public override bool Equals(object obj)
        {
            return obj is DamageResult other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BaseAmount, Resistance, FinalAmount);
        }
    }
}

