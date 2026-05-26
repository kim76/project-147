using System;
using System.Collections.Generic;

namespace Project147.GameCore.Combat
{
    public sealed class TowerLoadoutPlanSet
    {
        public TowerLoadoutPlanSet(IReadOnlyList<TowerLoadoutPlan> plans)
            : this(plans, 0)
        {
        }

        private TowerLoadoutPlanSet(IReadOnlyList<TowerLoadoutPlan> plans, int selectedIndex)
        {
            if (plans == null)
            {
                throw new ArgumentNullException(nameof(plans));
            }

            if (plans.Count == 0)
            {
                throw new ArgumentException("Tower loadout plan set requires at least one plan.", nameof(plans));
            }

            var planIds = new HashSet<string>();

            foreach (var plan in plans)
            {
                if (plan == null)
                {
                    throw new ArgumentNullException(nameof(plans), "Tower loadout plan set cannot contain null plans.");
                }

                if (!planIds.Add(plan.Id))
                {
                    throw new ArgumentException("Tower loadout plan set cannot contain duplicate plans.", nameof(plans));
                }
            }

            if (selectedIndex < 0 || selectedIndex >= plans.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(selectedIndex), "Selected loadout plan index is outside the plan set.");
            }

            Plans = new List<TowerLoadoutPlan>(plans);
            SelectedIndex = selectedIndex;
        }

        public IReadOnlyList<TowerLoadoutPlan> Plans { get; }

        public int SelectedIndex { get; }

        public TowerLoadoutPlan SelectedPlan
        {
            get { return Plans[SelectedIndex]; }
        }

        public TowerLoadoutPlanSet SelectNext()
        {
            return new TowerLoadoutPlanSet(Plans, (SelectedIndex + 1) % Plans.Count);
        }

        public TowerLoadoutPlanSet SelectPrevious()
        {
            return new TowerLoadoutPlanSet(Plans, (SelectedIndex + Plans.Count - 1) % Plans.Count);
        }
    }
}
