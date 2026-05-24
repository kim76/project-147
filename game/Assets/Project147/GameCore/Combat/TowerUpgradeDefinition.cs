using System;

namespace Project147.GameCore.Combat
{
    public sealed class TowerUpgradeDefinition
    {
        public TowerUpgradeDefinition(
            string id,
            int cost,
            float damageMultiplier,
            float fireRateMultiplier,
            float rangeBonus,
            float criticalChanceBonus,
            float criticalDamageMultiplierBonus)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Tower upgrade id is required.", nameof(id));
            }

            if (cost < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(cost), "Tower upgrade cost cannot be negative.");
            }

            if (damageMultiplier <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(damageMultiplier),
                    "Damage multiplier must be greater than zero.");
            }

            if (fireRateMultiplier <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(fireRateMultiplier),
                    "Fire rate multiplier must be greater than zero.");
            }

            if (rangeBonus < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rangeBonus), "Range bonus cannot be negative.");
            }

            if (criticalChanceBonus < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(criticalChanceBonus),
                    "Critical chance bonus cannot be negative.");
            }

            if (criticalDamageMultiplierBonus < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(criticalDamageMultiplierBonus),
                    "Critical damage multiplier bonus cannot be negative.");
            }

            Id = id;
            Cost = cost;
            DamageMultiplier = damageMultiplier;
            FireRateMultiplier = fireRateMultiplier;
            RangeBonus = rangeBonus;
            CriticalChanceBonus = criticalChanceBonus;
            CriticalDamageMultiplierBonus = criticalDamageMultiplierBonus;
        }

        public string Id { get; }

        public int Cost { get; }

        public float DamageMultiplier { get; }

        public float FireRateMultiplier { get; }

        public float RangeBonus { get; }

        public float CriticalChanceBonus { get; }

        public float CriticalDamageMultiplierBonus { get; }

        public TowerDefinition ApplyTo(TowerDefinition tower)
        {
            if (tower == null)
            {
                throw new ArgumentNullException(nameof(tower));
            }

            var criticalChance = tower.CriticalChance + CriticalChanceBonus;

            if (criticalChance > 1)
            {
                throw new InvalidOperationException("Tower upgrade would push critical chance above one.");
            }

            return new TowerDefinition(
                tower.Id,
                tower.Cost,
                tower.Range + RangeBonus,
                tower.FireRatePerSecond * FireRateMultiplier,
                tower.Damage * DamageMultiplier,
                tower.DamageType,
                tower.DefaultTargetingMode,
                criticalChance,
                tower.CriticalDamageMultiplier + CriticalDamageMultiplierBonus);
        }
    }
}
