using System;
using System.Collections.Generic;
using Project147.GameCore.Combat;

namespace Project147.GameCore.Abilities
{
    public sealed class OrbitalStrikeResolver
    {
        private readonly DamageResolver damageResolver;

        public OrbitalStrikeResolver(DamageResolver damageResolver)
        {
            this.damageResolver = damageResolver ?? throw new ArgumentNullException(nameof(damageResolver));
        }

        public IReadOnlyList<PlayerAbilityTargetResult> Resolve(
            PlayerAbilityDefinition ability,
            IReadOnlyList<AlienState> targets)
        {
            if (ability == null)
            {
                throw new ArgumentNullException(nameof(ability));
            }

            if (targets == null)
            {
                throw new ArgumentNullException(nameof(targets));
            }

            if (!ability.HasDamage)
            {
                throw new ArgumentException("Orbital strike ability requires damage.", nameof(ability));
            }

            var results = new List<PlayerAbilityTargetResult>();
            var damageRequest = new DamageRequest(ability.DamageAmount, ability.DamageType);

            foreach (var target in targets)
            {
                if (target == null)
                {
                    throw new ArgumentNullException(nameof(targets), "Orbital strike targets cannot contain null values.");
                }

                if (!target.IsAlive)
                {
                    continue;
                }

                var damage = damageResolver.Resolve(damageRequest, target.Definition);
                results.Add(new PlayerAbilityTargetResult(
                    target,
                    target.ApplyDamage(damage.FinalAmount)));
            }

            return results;
        }
    }
}
