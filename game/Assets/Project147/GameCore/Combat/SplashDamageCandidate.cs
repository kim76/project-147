using System;

namespace Project147.GameCore.Combat
{
    public readonly struct SplashDamageCandidate : IEquatable<SplashDamageCandidate>
    {
        public SplashDamageCandidate(AlienState target, float distanceFromImpact)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));

            if (distanceFromImpact < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(distanceFromImpact),
                    "Distance from impact cannot be negative.");
            }

            DistanceFromImpact = distanceFromImpact;
        }

        public AlienState Target { get; }

        public float DistanceFromImpact { get; }

        public bool Equals(SplashDamageCandidate other)
        {
            return ReferenceEquals(Target, other.Target)
                && DistanceFromImpact.Equals(other.DistanceFromImpact);
        }

        public override bool Equals(object obj)
        {
            return obj is SplashDamageCandidate other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Target, DistanceFromImpact);
        }
    }
}
