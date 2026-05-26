using System;

namespace Project147.GameCore.Level
{
    public sealed class BaseState
    {
        public BaseState(int maxHealth)
            : this(maxHealth, maxHealth, 0)
        {
        }

        private BaseState(int maxHealth, int currentHealth, int currentShield)
        {
            if (maxHealth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxHealth), "Base health must be greater than zero.");
            }

            if (currentHealth < 0 || currentHealth > maxHealth)
            {
                throw new ArgumentOutOfRangeException(nameof(currentHealth), "Current health must be between zero and max health.");
            }

            if (currentShield < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(currentShield), "Current shield cannot be negative.");
            }

            MaxHealth = maxHealth;
            CurrentHealth = currentHealth;
            CurrentShield = currentShield;
        }

        public int MaxHealth { get; }

        public int CurrentHealth { get; }

        public int CurrentShield { get; }

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

            var shieldDamage = Math.Min(CurrentShield, amount);
            var remainingDamage = amount - shieldDamage;
            return new BaseState(
                MaxHealth,
                Math.Max(0, CurrentHealth - remainingDamage),
                CurrentShield - shieldDamage);
        }

        public BaseState Repair(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Repair amount cannot be negative.");
            }

            return new BaseState(MaxHealth, Math.Min(MaxHealth, CurrentHealth + amount), CurrentShield);
        }

        public BaseState AddShield(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Shield amount cannot be negative.");
            }

            return new BaseState(MaxHealth, CurrentHealth, CurrentShield + amount);
        }
    }
}
