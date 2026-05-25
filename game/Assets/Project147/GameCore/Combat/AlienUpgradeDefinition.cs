using System;
using System.Collections.Generic;

namespace Project147.GameCore.Combat
{
    public sealed class AlienUpgradeDefinition
    {
        public AlienUpgradeDefinition(
            string id,
            float healthMultiplier,
            float speedMultiplier,
            float rewardMultiplier,
            float dodgeChanceBonus,
            DamageType resistanceDamageType,
            float resistanceBonus)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Alien upgrade id is required.", nameof(id));
            }

            if (healthMultiplier <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(healthMultiplier),
                    "Health multiplier must be greater than zero.");
            }

            if (speedMultiplier <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(speedMultiplier),
                    "Speed multiplier must be greater than zero.");
            }

            if (rewardMultiplier <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(rewardMultiplier),
                    "Reward multiplier must be greater than zero.");
            }

            if (dodgeChanceBonus < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(dodgeChanceBonus), "Dodge chance bonus cannot be negative.");
            }

            if (resistanceBonus < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(resistanceBonus), "Resistance bonus cannot be negative.");
            }

            Id = id;
            HealthMultiplier = healthMultiplier;
            SpeedMultiplier = speedMultiplier;
            RewardMultiplier = rewardMultiplier;
            DodgeChanceBonus = dodgeChanceBonus;
            ResistanceDamageType = resistanceDamageType;
            ResistanceBonus = resistanceBonus;
        }

        public string Id { get; }

        public float HealthMultiplier { get; }

        public float SpeedMultiplier { get; }

        public float RewardMultiplier { get; }

        public float DodgeChanceBonus { get; }

        public DamageType ResistanceDamageType { get; }

        public float ResistanceBonus { get; }

        public AlienDefinition ApplyTo(AlienDefinition alien)
        {
            if (alien == null)
            {
                throw new ArgumentNullException(nameof(alien));
            }

            var dodgeChance = alien.DodgeChance + DodgeChanceBonus;

            if (dodgeChance > 1)
            {
                throw new InvalidOperationException("Alien upgrade would push dodge chance above one.");
            }

            var resistances = CopyResistances(alien);
            var resistance = alien.GetResistance(ResistanceDamageType) + ResistanceBonus;

            if (resistance > 1)
            {
                throw new InvalidOperationException("Alien upgrade would push resistance above one.");
            }

            resistances[ResistanceDamageType] = resistance;

            return new AlienDefinition(
                alien.Id,
                alien.MaxHealth * HealthMultiplier,
                alien.SpeedCellsPerSecond * SpeedMultiplier,
                Convert.ToInt32(Math.Round(alien.Reward * RewardMultiplier, MidpointRounding.AwayFromZero)),
                resistances,
                dodgeChance);
        }

        private static Dictionary<DamageType, float> CopyResistances(AlienDefinition alien)
        {
            var resistances = new Dictionary<DamageType, float>();

            foreach (DamageType damageType in Enum.GetValues(typeof(DamageType)))
            {
                var resistance = alien.GetResistance(damageType);

                if (resistance > 0)
                {
                    resistances.Add(damageType, resistance);
                }
            }

            return resistances;
        }
    }
}
