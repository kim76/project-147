using System;

namespace Project147.GameCore.Combat
{
    public sealed class AttackResolver
    {
        private readonly DamageResolver damageResolver;

        public AttackResolver(DamageResolver damageResolver)
        {
            this.damageResolver = damageResolver ?? throw new ArgumentNullException(nameof(damageResolver));
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

            var damage = damageResolver.Resolve(
                new DamageRequest(tower.Damage, tower.DamageType),
                target.Definition);
            var updatedTarget = target.ApplyDamage(damage.FinalAmount);

            return new AttackResult(damage, updatedTarget, target.IsAlive && !updatedTarget.IsAlive);
        }
    }
}

