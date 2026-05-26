using System;
using System.Collections.Generic;

namespace Project147.GameCore.Combat
{
    public sealed class AlienUpgradeChoicePlan
    {
        public AlienUpgradeChoicePlan(int budget, IReadOnlyList<AlienUpgradeChoiceDefinition> choices)
        {
            if (budget < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(budget), "Alien upgrade budget cannot be negative.");
            }

            if (choices == null)
            {
                throw new ArgumentNullException(nameof(choices));
            }

            var totalCost = 0;

            foreach (var choice in choices)
            {
                if (choice == null)
                {
                    throw new ArgumentException("Alien upgrade choices cannot contain null entries.", nameof(choices));
                }

                totalCost += choice.Cost;
            }

            Budget = budget;
            Choices = new List<AlienUpgradeChoiceDefinition>(choices);
            TotalCost = totalCost;
        }

        public int Budget { get; }

        public IReadOnlyList<AlienUpgradeChoiceDefinition> Choices { get; }

        public int TotalCost { get; }

        public int RemainingBudget
        {
            get { return Budget - TotalCost; }
        }

        public bool IsWithinBudget
        {
            get { return TotalCost <= Budget; }
        }

        public AlienUpgradeChoicePlan Add(AlienUpgradeChoiceDefinition choice)
        {
            if (choice == null)
            {
                throw new ArgumentNullException(nameof(choice));
            }

            var choices = new List<AlienUpgradeChoiceDefinition>(Choices)
            {
                choice
            };

            return new AlienUpgradeChoicePlan(Budget, choices);
        }

        public AlienDefinition ApplyTo(AlienDefinition alien)
        {
            if (alien == null)
            {
                throw new ArgumentNullException(nameof(alien));
            }

            var current = alien;

            foreach (var choice in Choices)
            {
                current = choice.ApplyTo(current);
            }

            return current;
        }
    }
}
