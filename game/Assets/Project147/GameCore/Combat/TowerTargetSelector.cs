using System;
using System.Collections.Generic;

namespace Project147.GameCore.Combat
{
    public sealed class TowerTargetSelector
    {
        public TargetCandidate? SelectTarget(
            IEnumerable<TargetCandidate> candidates,
            TowerTargetingMode targetingMode)
        {
            if (!Enum.IsDefined(typeof(TowerTargetingMode), targetingMode))
            {
                throw new ArgumentOutOfRangeException(nameof(targetingMode), targetingMode, "Unknown targeting mode.");
            }

            if (candidates == null)
            {
                throw new ArgumentNullException(nameof(candidates));
            }

            TargetCandidate? selected = null;

            foreach (var candidate in candidates)
            {
                if (!candidate.Alien.IsAlive)
                {
                    continue;
                }

                if (!selected.HasValue || IsBetterCandidate(candidate, selected.Value, targetingMode))
                {
                    selected = candidate;
                }
            }

            return selected;
        }

        private static bool IsBetterCandidate(
            TargetCandidate candidate,
            TargetCandidate current,
            TowerTargetingMode targetingMode)
        {
            switch (targetingMode)
            {
                case TowerTargetingMode.First:
                    return candidate.PathProgress > current.PathProgress;
                case TowerTargetingMode.Last:
                    return candidate.PathProgress < current.PathProgress;
                case TowerTargetingMode.Closest:
                    return candidate.DistanceToTower < current.DistanceToTower;
                case TowerTargetingMode.Strongest:
                    return candidate.Alien.CurrentHealth > current.Alien.CurrentHealth;
                case TowerTargetingMode.Weakest:
                    return candidate.Alien.CurrentHealth < current.Alien.CurrentHealth;
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetingMode), targetingMode, "Unknown targeting mode.");
            }
        }
    }
}
