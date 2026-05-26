using System;
using Project147.GameCore.Level;

namespace Project147.GameCore.Abilities
{
    public sealed class BaseShieldBurstResolver
    {
        public BaseState Resolve(PlayerAbilityDefinition ability, BaseState target)
        {
            if (ability == null)
            {
                throw new ArgumentNullException(nameof(ability));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (!ability.HasBaseShield)
            {
                throw new ArgumentException("Base shield burst ability requires base shield amount.", nameof(ability));
            }

            return target.AddShield(ability.BaseShieldAmount);
        }
    }
}
