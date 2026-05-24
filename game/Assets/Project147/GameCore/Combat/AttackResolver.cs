using System;

namespace Project147.GameCore.Combat
{
    public sealed class AttackResolver
    {
        private readonly DamageResolver damageResolver;
        private readonly ICombatChanceRoller chanceRoller;

        public AttackResolver(DamageResolver damageResolver)
            : this(damageResolver, new RandomCombatChanceRoller())
        {
        }

        public AttackResolver(DamageResolver damageResolver, ICombatChanceRoller chanceRoller)
        {
            this.damageResolver = damageResolver ?? throw new ArgumentNullException(nameof(damageResolver));
            this.chanceRoller = chanceRoller ?? throw new ArgumentNullException(nameof(chanceRoller));
        }

        public AttackResult Resolve(TowerDefinition tower, AlienState target)
        {
            if (tower == null)
            {
                throw new ArgumentNullException(nameof(tower));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (!target.IsAlive)
            {
                return new AttackResult(
                    new DamageResult(tower.Damage, target.Definition.GetResistance(tower.DamageType), 0),
                    target,
                    false);
            }

            if (chanceRoller.Succeeds(target.Definition.DodgeChance))
            {
                return new AttackResult(
                    new DamageResult(tower.Damage, target.Definition.GetResistance(tower.DamageType), 0, false, true),
                    target,
                    false);
            }

            var wasCritical = chanceRoller.Succeeds(tower.CriticalChance);
            var baseDamage = wasCritical ? tower.Damage * tower.CriticalDamageMultiplier : tower.Damage;
            var damage = damageResolver.Resolve(
                new DamageRequest(baseDamage, tower.DamageType),
                target.Definition);
            damage = new DamageResult(
                damage.BaseAmount,
                damage.Resistance,
                damage.FinalAmount,
                wasCritical,
                false);
            var updatedTarget = target.ApplyDamage(damage.FinalAmount);

            return new AttackResult(damage, updatedTarget, target.IsAlive && !updatedTarget.IsAlive);
        }
    }
}
