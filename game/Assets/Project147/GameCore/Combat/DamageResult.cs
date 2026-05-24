using System;

namespace Project147.GameCore.Combat
{
    public readonly struct DamageResult : IEquatable<DamageResult>
    {
        public DamageResult(
            float baseAmount,
            float resistance,
            float finalAmount,
            bool wasCritical = false,
            bool wasDodged = false)
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

            if (wasCritical && wasDodged)
            {
                throw new ArgumentException("Damage cannot be both critical and dodged.");
            }

            if (wasDodged && finalAmount > 0)
            {
                throw new ArgumentException("Dodged damage must have zero final amount.");
            }

            BaseAmount = baseAmount;
            Resistance = resistance;
            FinalAmount = finalAmount;
            WasCritical = wasCritical;
            WasDodged = wasDodged;
        }

        public float BaseAmount { get; }

        public float Resistance { get; }

        public float FinalAmount { get; }

        public bool WasCritical { get; }

        public bool WasDodged { get; }

        public bool Equals(DamageResult other)
        {
            return BaseAmount.Equals(other.BaseAmount)
                && Resistance.Equals(other.Resistance)
                && FinalAmount.Equals(other.FinalAmount)
                && WasCritical == other.WasCritical
                && WasDodged == other.WasDodged;
        }

        public override bool Equals(object obj)
        {
            return obj is DamageResult other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BaseAmount, Resistance, FinalAmount, WasCritical, WasDodged);
        }
    }
}
