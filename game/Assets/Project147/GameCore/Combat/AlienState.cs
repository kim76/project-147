using System;

namespace Project147.GameCore.Combat
{
    public sealed class AlienState
    {
        public AlienState(AlienDefinition definition)
            : this(definition, definition?.MaxHealth ?? 0)
        {
        }

        private AlienState(AlienDefinition definition, float currentHealth)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));

            if (currentHealth < 0 || currentHealth > definition.MaxHealth)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(currentHealth),
                    "Alien health must be between zero and max health.");
            }

            CurrentHealth = currentHealth;
        }

        public AlienDefinition Definition { get; }

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
            return new AlienState(Definition, nextHealth);
        }

        public AlienState Heal(float amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Heal amount cannot be negative.");
            }

            var nextHealth = Math.Min(Definition.MaxHealth, CurrentHealth + amount);
            return new AlienState(Definition, nextHealth);
        }
    }
}

