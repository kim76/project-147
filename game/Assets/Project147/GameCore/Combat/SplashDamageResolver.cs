using System;
using System.Collections.Generic;

namespace Project147.GameCore.Combat
{
    public sealed class SplashDamageResolver
    {
        private readonly DamageResolver damageResolver;

        public SplashDamageResolver(DamageResolver damageResolver)
        {
            this.damageResolver = damageResolver ?? throw new ArgumentNullException(nameof(damageResolver));
        }

        public IReadOnlyList<SplashDamageResult> Resolve(
            TowerDefinition tower,
            IReadOnlyList<SplashDamageCandidate> candidates)
        {
            if (tower == null)
            {
                throw new ArgumentNullException(nameof(tower));
            }

            if (candidates == null)
            {
                throw new ArgumentNullException(nameof(candidates));
            }

            var results = new List<SplashDamageResult>();

            if (tower.SplashRadius <= 0 || tower.SplashDamageMultiplier <= 0)
            {
                return results;
            }

            var damageRequest = new DamageRequest(tower.Damage * tower.SplashDamageMultiplier, tower.DamageType);

            foreach (var candidate in candidates)
            {
                if (!candidate.Target.IsAlive || candidate.DistanceFromImpact > tower.SplashRadius)
                {
                    continue;
                }

                var damage = damageResolver.Resolve(damageRequest, candidate.Target.Definition);
                var damagedTarget = candidate.Target.ApplyDamage(damage.FinalAmount);
                results.Add(new SplashDamageResult(candidate.Target, damagedTarget, damage));
            }

            return results;
        }
    }
}
