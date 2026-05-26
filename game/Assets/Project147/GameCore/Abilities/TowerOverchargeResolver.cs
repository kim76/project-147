using System;
using Project147.GameCore.Choices;

namespace Project147.GameCore.Abilities
{
    public sealed class TowerOverchargeResolver
    {
        public RunModifierState Resolve(PlayerAbilityDefinition definition, RunModifierState modifiers)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            if (modifiers == null)
            {
                throw new ArgumentNullException(nameof(modifiers));
            }

            if (!definition.HasTowerOvercharge)
            {
                throw new ArgumentException("Ability does not overcharge towers.", nameof(definition));
            }

            return modifiers
                .AddActiveWaveTowerDamagePercent(definition.TowerDamagePercent)
                .AddActiveWaveTowerFireRatePercent(definition.TowerFireRatePercent);
        }
    }
}
