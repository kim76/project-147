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
            : this(id, cooldownSeconds, statusEffect, 0, DamageType.Kinetic)
        {
            if (statusEffect == null)
            {
                throw new ArgumentNullException(nameof(statusEffect));
            }
        }

        public PlayerAbilityDefinition(
            string id,
            float cooldownSeconds,
            float damageAmount,
            DamageType damageType)
            : this(id, cooldownSeconds, null, damageAmount, damageType)
        {
            if (damageAmount <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(damageAmount),
                    "Player ability damage must be greater than zero.");
            }
        }

        private PlayerAbilityDefinition(
            string id,
            float cooldownSeconds,
            AlienStatusEffectDefinition statusEffect,
            float damageAmount,
            DamageType damageType)
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

            if (damageAmount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(damageAmount),
                    "Player ability damage cannot be negative.");
            }

            Id = id;
            CooldownSeconds = cooldownSeconds;
            StatusEffect = statusEffect;
            DamageAmount = damageAmount;
            DamageType = damageType;
        }

        public string Id { get; }

        public float CooldownSeconds { get; }

        public AlienStatusEffectDefinition StatusEffect { get; }

        public float DamageAmount { get; }

        public DamageType DamageType { get; }

        public bool HasStatusEffect
        {
            get { return StatusEffect != null; }
        }

        public bool HasDamage
        {
            get { return DamageAmount > 0; }
        }
    }
}
