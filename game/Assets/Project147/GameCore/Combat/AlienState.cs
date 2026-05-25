using System;

namespace Project147.GameCore.Combat
{
    public sealed class AlienState
    {
        public AlienState(AlienDefinition definition)
            : this(definition, 1, definition?.MaxHealth ?? 0)
        {
        }

        private AlienState(AlienDefinition definition, int level, float currentHealth)
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

            Level = level;
            CurrentHealth = currentHealth;
        }

        public AlienDefinition Definition { get; }

        public int Level { get; }

        public float CurrentHealth { get; }

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

            var nextHealth = Math.Max(0, CurrentHealth - damage);
            return new AlienState(Definition, Level, nextHealth);
        }

        public AlienState Heal(float amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Heal amount cannot be negative.");
            }

            var nextHealth = Math.Min(Definition.MaxHealth, CurrentHealth + amount);
            return new AlienState(Definition, Level, nextHealth);
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
            return new AlienState(upgradedDefinition, Level + 1, upgradedHealth);
        }
    }
}
