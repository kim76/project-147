using System;

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
            TowerTargetingMode defaultTargetingMode)
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

            Id = id;
            Cost = cost;
            Range = range;
            FireRatePerSecond = fireRatePerSecond;
            Damage = damage;
            DamageType = damageType;
            DefaultTargetingMode = defaultTargetingMode;
        }

        public string Id { get; }

        public int Cost { get; }

        public float Range { get; }

        public float FireRatePerSecond { get; }

        public float Damage { get; }

        public DamageType DamageType { get; }

        public TowerTargetingMode DefaultTargetingMode { get; }
    }
}

