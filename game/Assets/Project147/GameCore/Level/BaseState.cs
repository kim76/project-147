using System;

namespace Project147.GameCore.Level
{
    public sealed class BaseState
    {
        public BaseState(int maxHealth)
            : this(maxHealth, maxHealth)
        {
        }

        private BaseState(int maxHealth, int currentHealth)
        {
            if (maxHealth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxHealth), "Base health must be greater than zero.");
            }

            if (currentHealth < 0 || currentHealth > maxHealth)
            {
                throw new ArgumentOutOfRangeException(nameof(currentHealth), "Current health must be between zero and max health.");
            }

            MaxHealth = maxHealth;
            CurrentHealth = currentHealth;
        }

        public int MaxHealth { get; }

        public int CurrentHealth { get; }

        public bool IsDestroyed
        {
            get { return CurrentHealth <= 0; }
        }

        public BaseState ApplyLeakDamage(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Leak damage cannot be negative.");
            }

            return new BaseState(MaxHealth, Math.Max(0, CurrentHealth - amount));
        }
    }
}

