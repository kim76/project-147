using System;

namespace Project147.GameCore.Level
{
    public sealed class CurrencyWallet
    {
        public CurrencyWallet(int balance)
        {
            if (balance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(balance), "Currency balance cannot be negative.");
            }

            Balance = balance;
        }

        public int Balance { get; }

        public bool CanSpend(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Spend amount cannot be negative.");
            }

            return Balance >= amount;
        }

        public CurrencyWallet Spend(int amount)
        {
            if (!CanSpend(amount))
            {
                throw new InvalidOperationException("Not enough currency.");
            }

            return new CurrencyWallet(Balance - amount);
        }

        public CurrencyWallet Add(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Added amount cannot be negative.");
            }

            return new CurrencyWallet(Balance + amount);
        }
    }
}

