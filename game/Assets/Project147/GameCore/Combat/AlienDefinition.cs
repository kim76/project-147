using System;
using System.Collections.Generic;

namespace Project147.GameCore.Combat
{
    public sealed class AlienDefinition
    {
        private readonly IReadOnlyDictionary<DamageType, float> resistances;

        public AlienDefinition(
            string id,
            float maxHealth,
            float speedCellsPerSecond,
            int reward,
            IReadOnlyDictionary<DamageType, float> resistances,
            float dodgeChance = 0,
            float shieldCapacity = 0)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Alien id is required.", nameof(id));
            }

            if (maxHealth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxHealth), "Alien health must be greater than zero.");
            }

            if (speedCellsPerSecond <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(speedCellsPerSecond), "Alien speed must be greater than zero.");
            }

            if (reward < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(reward), "Alien reward cannot be negative.");
            }

            if (resistances == null)
            {
                throw new ArgumentNullException(nameof(resistances));
            }

            foreach (var resistance in resistances)
            {
                if (resistance.Value < 0 || resistance.Value > 1)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(resistances),
                    "Alien resistance values must be between zero and one.");
                }
            }

            if (dodgeChance < 0 || dodgeChance > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(dodgeChance), "Alien dodge chance must be between zero and one.");
            }

            if (shieldCapacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(shieldCapacity), "Alien shield capacity cannot be negative.");
            }

            Id = id;
            MaxHealth = maxHealth;
            SpeedCellsPerSecond = speedCellsPerSecond;
            Reward = reward;
            DodgeChance = dodgeChance;
            ShieldCapacity = shieldCapacity;
            this.resistances = new Dictionary<DamageType, float>(resistances);
        }

        public string Id { get; }

        public float MaxHealth { get; }

        public float SpeedCellsPerSecond { get; }

        public int Reward { get; }

        public float DodgeChance { get; }

        public float ShieldCapacity { get; }

        public float GetResistance(DamageType damageType)
        {
            return resistances.TryGetValue(damageType, out var resistance) ? resistance : 0;
        }
    }
}
