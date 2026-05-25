using System;
using Project147.GameCore.Combat;

namespace Project147.GameCore.Abilities
{
    public sealed class PlayerAbilityDefinition
    {
        public PlayerAbilityDefinition(
            string id,
            float cooldownSeconds,
            AlienStatusEffectDefinition statusEffect)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Player ability id is required.", nameof(id));
            }

            if (cooldownSeconds <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(cooldownSeconds),
                    "Player ability cooldown must be greater than zero.");
            }

            Id = id;
            CooldownSeconds = cooldownSeconds;
            StatusEffect = statusEffect ?? throw new ArgumentNullException(nameof(statusEffect));
        }

        public string Id { get; }

        public float CooldownSeconds { get; }

        public AlienStatusEffectDefinition StatusEffect { get; }
    }
}
