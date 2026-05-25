using System;

namespace Project147.GameCore.Combat
{
    public readonly struct TargetCandidate : IEquatable<TargetCandidate>
    {
        public TargetCandidate(AlienState alien, float pathProgress, float distanceToTower)
        {
            Alien = alien ?? throw new ArgumentNullException(nameof(alien));

            if (pathProgress < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pathProgress), "Path progress cannot be negative.");
            }

            if (distanceToTower < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(distanceToTower), "Distance to tower cannot be negative.");
            }

            PathProgress = pathProgress;
            DistanceToTower = distanceToTower;
        }

        public AlienState Alien { get; }

        public float PathProgress { get; }

        public float DistanceToTower { get; }

        public bool IsTargetable
        {
            get { return PathProgress >= Alien.Definition.TargetableAfterPathProgress; }
        }

        public bool Equals(TargetCandidate other)
        {
            return ReferenceEquals(Alien, other.Alien)
                && PathProgress.Equals(other.PathProgress)
                && DistanceToTower.Equals(other.DistanceToTower);
        }

        public override bool Equals(object obj)
        {
            return obj is TargetCandidate other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Alien, PathProgress, DistanceToTower);
        }
    }
}
