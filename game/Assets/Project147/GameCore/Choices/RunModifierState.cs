using System;

namespace Project147.GameCore.Choices
{
    public sealed class RunModifierState
    {
        public RunModifierState()
            : this(0)
        {
        }

        private RunModifierState(int nextTowerDiscountAmount)
        {
            if (nextTowerDiscountAmount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(nextTowerDiscountAmount),
                    "Next tower discount cannot be negative.");
            }

            NextTowerDiscountAmount = nextTowerDiscountAmount;
        }

        public int NextTowerDiscountAmount { get; }

        public bool HasNextTowerDiscount
        {
            get { return NextTowerDiscountAmount > 0; }
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

            return new RunModifierState(NextTowerDiscountAmount + amount);
        }

        public RunModifierState ConsumeNextTowerDiscount()
        {
            return new RunModifierState();
        }
    }
}
