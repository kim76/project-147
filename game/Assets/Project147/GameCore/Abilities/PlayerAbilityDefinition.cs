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
            : this(id, cooldownSeconds, statusEffect, 0, DamageType.Kinetic, 0, 0, 0)
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
            : this(id, cooldownSeconds, null, damageAmount, damageType, 0, 0, 0)
        {
            if (damageAmount <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(damageAmount),
                    "Player ability damage must be greater than zero.");
            }
        }

        public PlayerAbilityDefinition(
            string id,
            float cooldownSeconds,
            int baseShieldAmount)
            : this(id, cooldownSeconds, null, 0, DamageType.Kinetic, baseShieldAmount, 0, 0)
        {
            if (baseShieldAmount <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(baseShieldAmount),
                    "Player ability base shield amount must be greater than zero.");
            }
        }

        public PlayerAbilityDefinition(
            string id,
            float cooldownSeconds,
            int towerDamagePercent,
            int towerFireRatePercent)
            : this(id, cooldownSeconds, null, 0, DamageType.Kinetic, 0, towerDamagePercent, towerFireRatePercent)
        {
            if (towerDamagePercent <= 0 && towerFireRatePercent <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(towerDamagePercent),
                    "Player ability tower overcharge must improve damage or fire rate.");
            }
        }

        private PlayerAbilityDefinition(
            string id,
            float cooldownSeconds,
            AlienStatusEffectDefinition statusEffect,
            float damageAmount,
            DamageType damageType,
            int baseShieldAmount,
            int towerDamagePercent,
            int towerFireRatePercent)
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

            if (baseShieldAmount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(baseShieldAmount),
                    "Player ability base shield amount cannot be negative.");
            }

            if (towerDamagePercent < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(towerDamagePercent),
                    "Player ability tower damage percent cannot be negative.");
            }

            if (towerFireRatePercent < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(towerFireRatePercent),
                    "Player ability tower fire-rate percent cannot be negative.");
            }

            Id = id;
            CooldownSeconds = cooldownSeconds;
            StatusEffect = statusEffect;
            DamageAmount = damageAmount;
            DamageType = damageType;
            BaseShieldAmount = baseShieldAmount;
            TowerDamagePercent = towerDamagePercent;
            TowerFireRatePercent = towerFireRatePercent;
        }

        public string Id { get; }

        public float CooldownSeconds { get; }

        public AlienStatusEffectDefinition StatusEffect { get; }

        public float DamageAmount { get; }

        public DamageType DamageType { get; }

        public int BaseShieldAmount { get; }

        public int TowerDamagePercent { get; }

        public int TowerFireRatePercent { get; }

        public bool HasStatusEffect
        {
            get { return StatusEffect != null; }
        }

        public bool HasDamage
        {
            get { return DamageAmount > 0; }
        }

        public bool HasBaseShield
        {
            get { return BaseShieldAmount > 0; }
        }

        public bool HasTowerOvercharge
        {
            get { return TowerDamagePercent > 0 || TowerFireRatePercent > 0; }
        }
    }
}
