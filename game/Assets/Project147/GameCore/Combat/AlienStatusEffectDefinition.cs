using System;

namespace Project147.GameCore.Combat
{
    public sealed class AlienStatusEffectDefinition
    {
        public AlienStatusEffectDefinition(
            string id,
            AlienStatusEffectType type,
            float durationSeconds,
            float movementSpeedMultiplier,
            float damagePerSecond = 0)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Alien status effect id is required.", nameof(id));
            }

            if (durationSeconds <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(durationSeconds),
                    "Status effect duration must be greater than zero.");
            }

            if (movementSpeedMultiplier <= 0 || movementSpeedMultiplier > 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(movementSpeedMultiplier),
                    "Movement speed multiplier must be greater than zero and no more than one.");
            }

            if (damagePerSecond < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(damagePerSecond), "Damage per second cannot be negative.");
            }

            Id = id;
            Type = type;
            DurationSeconds = durationSeconds;
            MovementSpeedMultiplier = movementSpeedMultiplier;
            DamagePerSecond = damagePerSecond;
        }

        public string Id { get; }

        public AlienStatusEffectType Type { get; }

        public float DurationSeconds { get; }

        public float MovementSpeedMultiplier { get; }

        public float DamagePerSecond { get; }
    }
}
