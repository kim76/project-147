using System;
using System.Collections.Generic;

namespace Project147.GameCore.Combat
{
    public sealed class TowerDefinition
    {
        public TowerDefinition(
            string id,
            int cost,
            float range,
            float fireRatePerSecond,
            float damage,
            DamageType damageType,
            TowerTargetingMode defaultTargetingMode,
            float criticalChance = 0,
            float criticalDamageMultiplier = 1,
            IReadOnlyList<AlienStatusEffectDefinition> statusEffects = null)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Tower id is required.", nameof(id));
            }

            if (cost < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(cost), "Tower cost cannot be negative.");
            }

            if (range <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(range), "Tower range must be greater than zero.");
            }

            if (fireRatePerSecond <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(fireRatePerSecond), "Tower fire rate must be greater than zero.");
            }

            if (damage <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(damage), "Tower damage must be greater than zero.");
            }

            if (criticalChance < 0 || criticalChance > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(criticalChance), "Critical chance must be between zero and one.");
            }

            if (criticalDamageMultiplier < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(criticalDamageMultiplier),
                    "Critical damage multiplier must be at least one.");
            }

            statusEffects = statusEffects ?? new List<AlienStatusEffectDefinition>();

            foreach (var statusEffect in statusEffects)
            {
                if (statusEffect == null)
                {
                    throw new ArgumentNullException(nameof(statusEffects), "Tower status effects cannot contain null values.");
                }
            }

            Id = id;
            Cost = cost;
            Range = range;
            FireRatePerSecond = fireRatePerSecond;
            Damage = damage;
            DamageType = damageType;
            DefaultTargetingMode = defaultTargetingMode;
            CriticalChance = criticalChance;
            CriticalDamageMultiplier = criticalDamageMultiplier;
            StatusEffects = new List<AlienStatusEffectDefinition>(statusEffects);
        }

        public string Id { get; }

        public int Cost { get; }

        public float Range { get; }

        public float FireRatePerSecond { get; }

        public float Damage { get; }

        public DamageType DamageType { get; }

        public TowerTargetingMode DefaultTargetingMode { get; }

        public float CriticalChance { get; }

        public float CriticalDamageMultiplier { get; }

        public IReadOnlyList<AlienStatusEffectDefinition> StatusEffects { get; }
    }
}
