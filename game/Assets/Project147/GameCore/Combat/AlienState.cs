using System;
using System.Collections.Generic;

namespace Project147.GameCore.Combat
{
    public sealed class AlienState
    {
        public AlienState(AlienDefinition definition)
            : this(
                definition,
                1,
                definition?.MaxHealth ?? 0,
                definition?.ShieldCapacity ?? 0,
                new List<AlienStatusEffectState>())
        {
        }

        private AlienState(
            AlienDefinition definition,
            int level,
            float currentHealth,
            float currentShield,
            IReadOnlyList<AlienStatusEffectState> activeStatusEffects)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));

            if (level <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(level), "Alien level must be greater than zero.");
            }

            if (currentHealth < 0 || currentHealth > definition.MaxHealth)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(currentHealth),
                    "Alien health must be between zero and max health.");
            }

            if (currentShield < 0 || currentShield > definition.ShieldCapacity)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(currentShield),
                    "Alien shield must be between zero and shield capacity.");
            }

            if (activeStatusEffects == null)
            {
                throw new ArgumentNullException(nameof(activeStatusEffects));
            }

            Level = level;
            CurrentHealth = currentHealth;
            CurrentShield = currentShield;
            ActiveStatusEffects = new List<AlienStatusEffectState>(activeStatusEffects);
        }

        public AlienDefinition Definition { get; }

        public int Level { get; }

        public float CurrentHealth { get; }

        public float CurrentShield { get; }

        public IReadOnlyList<AlienStatusEffectState> ActiveStatusEffects { get; }

        public float MovementSpeedMultiplier
        {
            get
            {
                var multiplier = 1f;

                foreach (var effect in ActiveStatusEffects)
                {
                    if (effect.Definition.Type == AlienStatusEffectType.Slow)
                    {
                        multiplier = Math.Min(multiplier, effect.Definition.MovementSpeedMultiplier);
                    }
                }

                return multiplier;
            }
        }

        public bool IsAlive
        {
            get { return CurrentHealth > 0; }
        }

        public AlienState ApplyDamage(float damage)
        {
            if (damage < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(damage), "Damage cannot be negative.");
            }

            var shieldDamage = Math.Min(CurrentShield, damage);
            var remainingDamage = damage - shieldDamage;
            var nextShield = CurrentShield - shieldDamage;
            var nextHealth = Math.Max(0, CurrentHealth - remainingDamage);
            return new AlienState(Definition, Level, nextHealth, nextShield, ActiveStatusEffects);
        }

        public AlienState Heal(float amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Heal amount cannot be negative.");
            }

            var nextHealth = Math.Min(Definition.MaxHealth, CurrentHealth + amount);
            return new AlienState(Definition, Level, nextHealth, CurrentShield, ActiveStatusEffects);
        }

        public AlienState Upgrade(AlienUpgradeDefinition upgrade)
        {
            if (upgrade == null)
            {
                throw new ArgumentNullException(nameof(upgrade));
            }

            var upgradedDefinition = upgrade.ApplyTo(Definition);
            var missingHealth = Definition.MaxHealth - CurrentHealth;
            var upgradedHealth = IsAlive ? Math.Max(0, upgradedDefinition.MaxHealth - missingHealth) : 0;
            var upgradedShield = Math.Min(CurrentShield, upgradedDefinition.ShieldCapacity);
            return new AlienState(upgradedDefinition, Level + 1, upgradedHealth, upgradedShield, ActiveStatusEffects);
        }

        public AlienState ApplyStatusEffect(AlienStatusEffectDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            var effects = new List<AlienStatusEffectState>(ActiveStatusEffects)
            {
                new AlienStatusEffectState(definition)
            };

            return new AlienState(Definition, Level, CurrentHealth, CurrentShield, effects);
        }

        public AlienState TickStatusEffects(float deltaSeconds)
        {
            if (deltaSeconds < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(deltaSeconds), "Delta seconds cannot be negative.");
            }

            if (ActiveStatusEffects.Count == 0)
            {
                return this;
            }

            var effects = new List<AlienStatusEffectState>();

            foreach (var effect in ActiveStatusEffects)
            {
                var updatedEffect = effect.Tick(deltaSeconds);

                if (!updatedEffect.IsExpired)
                {
                    effects.Add(updatedEffect);
                }
            }

            return new AlienState(Definition, Level, CurrentHealth, CurrentShield, effects);
        }
    }
}
