using System;

namespace Project147.GameCore.Choices
{
    public sealed class RunModifierState
    {
        public RunModifierState()
            : this(0, 0, 0)
        {
        }

        private RunModifierState(
            int nextTowerDiscountAmount,
            int pendingWaveTowerDamagePercent,
            int activeWaveTowerDamagePercent)
        {
            if (nextTowerDiscountAmount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(nextTowerDiscountAmount),
                    "Next tower discount cannot be negative.");
            }

            if (pendingWaveTowerDamagePercent < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(pendingWaveTowerDamagePercent),
                    "Pending wave tower damage percent cannot be negative.");
            }

            if (activeWaveTowerDamagePercent < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(activeWaveTowerDamagePercent),
                    "Active wave tower damage percent cannot be negative.");
            }

            NextTowerDiscountAmount = nextTowerDiscountAmount;
            PendingWaveTowerDamagePercent = pendingWaveTowerDamagePercent;
            ActiveWaveTowerDamagePercent = activeWaveTowerDamagePercent;
        }

        public int NextTowerDiscountAmount { get; }

        public int PendingWaveTowerDamagePercent { get; }

        public int ActiveWaveTowerDamagePercent { get; }

        public bool HasNextTowerDiscount
        {
            get { return NextTowerDiscountAmount > 0; }
        }

        public bool HasPendingWaveTowerDamageBoost
        {
            get { return PendingWaveTowerDamagePercent > 0; }
        }

        public bool HasActiveWaveTowerDamageBoost
        {
            get { return ActiveWaveTowerDamagePercent > 0; }
        }

        public float ActiveWaveTowerDamageMultiplier
        {
            get { return 1 + ActiveWaveTowerDamagePercent / 100f; }
        }

        public int CalculateTowerCost(int originalCost)
        {
            if (originalCost < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(originalCost), "Tower cost cannot be negative.");
            }

            return Math.Max(0, originalCost - NextTowerDiscountAmount);
        }

        public RunModifierState AddNextTowerDiscount(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Next tower discount cannot be negative.");
            }

            return new RunModifierState(
                NextTowerDiscountAmount + amount,
                PendingWaveTowerDamagePercent,
                ActiveWaveTowerDamagePercent);
        }

        public RunModifierState AddNextWaveTowerDamagePercent(int percent)
        {
            if (percent < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(percent),
                    "Next wave tower damage percent cannot be negative.");
            }

            return new RunModifierState(
                NextTowerDiscountAmount,
                PendingWaveTowerDamagePercent + percent,
                ActiveWaveTowerDamagePercent);
        }

        public RunModifierState ConsumeNextTowerDiscount()
        {
            return new RunModifierState(
                0,
                PendingWaveTowerDamagePercent,
                ActiveWaveTowerDamagePercent);
        }

        public RunModifierState StartWave()
        {
            return new RunModifierState(NextTowerDiscountAmount, 0, PendingWaveTowerDamagePercent);
        }

        public RunModifierState EndWave()
        {
            return new RunModifierState(NextTowerDiscountAmount, PendingWaveTowerDamagePercent, 0);
        }
    }
}
