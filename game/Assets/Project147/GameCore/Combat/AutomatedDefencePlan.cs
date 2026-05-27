using System;
using System.Collections.Generic;

namespace Project147.GameCore.Combat
{
    public sealed class AutomatedDefencePlan
    {
        public AutomatedDefencePlan(int budget, IReadOnlyList<AutomatedDefencePlacement> placements)
        {
            if (budget < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(budget), "Automated defence budget cannot be negative.");
            }

            if (placements == null)
            {
                throw new ArgumentNullException(nameof(placements));
            }

            var totalCost = 0;

            foreach (var placement in placements)
            {
                if (placement == null)
                {
                    throw new ArgumentException("Automated defence placements cannot contain null entries.", nameof(placements));
                }

                totalCost += placement.Cost;
            }

            Budget = budget;
            Placements = new List<AutomatedDefencePlacement>(placements);
            TotalCost = totalCost;
        }

        public int Budget { get; }

        public IReadOnlyList<AutomatedDefencePlacement> Placements { get; }

        public int TotalCost { get; }

        public int RemainingBudget
        {
            get { return Budget - TotalCost; }
        }

        public bool IsWithinBudget
        {
            get { return TotalCost <= Budget; }
        }
    }
}
