using System;

namespace Project147.GameCore.Grid
{
    public readonly struct TowerPlacementResult : IEquatable<TowerPlacementResult>
    {
        private TowerPlacementResult(bool isValid, TowerPlacementFailureReason failureReason)
        {
            IsValid = isValid;
            FailureReason = failureReason;
        }

        public bool IsValid { get; }

        public TowerPlacementFailureReason FailureReason { get; }

        public static TowerPlacementResult Valid()
        {
            return new TowerPlacementResult(true, TowerPlacementFailureReason.None);
        }

        public static TowerPlacementResult Invalid(TowerPlacementFailureReason failureReason)
        {
            if (failureReason == TowerPlacementFailureReason.None)
            {
                throw new ArgumentException("Invalid placement requires a failure reason.", nameof(failureReason));
            }

            return new TowerPlacementResult(false, failureReason);
        }

        public bool Equals(TowerPlacementResult other)
        {
            return IsValid == other.IsValid && FailureReason == other.FailureReason;
        }

        public override bool Equals(object obj)
        {
            return obj is TowerPlacementResult other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IsValid, FailureReason);
        }
    }
}

