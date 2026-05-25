using System;
using System.Collections.Generic;
using Project147.GameCore.Combat;

namespace Project147.GameCore.Abilities
{
    public sealed class FreezePulseResolver
    {
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

            var results = new List<PlayerAbilityTargetResult>();

            foreach (var target in targets)
            {
                if (target == null)
                {
                    throw new ArgumentNullException(nameof(targets), "Freeze pulse targets cannot contain null values.");
                }

                if (!target.IsAlive)
                {
                    continue;
                }

                results.Add(new PlayerAbilityTargetResult(
                    target,
                    target.ApplyStatusEffect(ability.StatusEffect)));
            }

            return results;
        }
    }
}
