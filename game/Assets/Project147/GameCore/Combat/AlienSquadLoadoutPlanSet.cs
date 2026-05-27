using System;
using System.Collections.Generic;

namespace Project147.GameCore.Combat
{
    public sealed class AlienSquadLoadoutPlanSet
    {
        public AlienSquadLoadoutPlanSet(IReadOnlyList<AlienSquadLoadoutPlan> plans)
            : this(plans, 0)
        {
        }

        private AlienSquadLoadoutPlanSet(IReadOnlyList<AlienSquadLoadoutPlan> plans, int selectedIndex)
        {
            if (plans == null)
            {
                throw new ArgumentNullException(nameof(plans));
            }

            if (plans.Count == 0)
            {
                throw new ArgumentException("Alien squad loadout plan set requires at least one plan.", nameof(plans));
            }

            var planIds = new HashSet<string>();

            foreach (var plan in plans)
            {
                if (plan == null)
                {
                    throw new ArgumentNullException(nameof(plans), "Alien squad loadout plan set cannot contain null plans.");
                }

                if (!planIds.Add(plan.Id))
                {
                    throw new ArgumentException("Alien squad loadout plan set cannot contain duplicate plans.", nameof(plans));
                }
            }

            if (selectedIndex < 0 || selectedIndex >= plans.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(selectedIndex), "Selected alien squad plan index is outside the plan set.");
            }

            Plans = new List<AlienSquadLoadoutPlan>(plans);
            SelectedIndex = selectedIndex;
        }

        public IReadOnlyList<AlienSquadLoadoutPlan> Plans { get; }

        public int SelectedIndex { get; }

        public AlienSquadLoadoutPlan SelectedPlan
        {
            get { return Plans[SelectedIndex]; }
        }

        public AlienSquadLoadout SelectedLoadout
        {
            get { return SelectedPlan.Loadout; }
        }

        public AlienSquadLoadoutPlanSet SelectNext()
        {
            return new AlienSquadLoadoutPlanSet(Plans, (SelectedIndex + 1) % Plans.Count);
        }

        public AlienSquadLoadoutPlanSet SelectPrevious()
        {
            return new AlienSquadLoadoutPlanSet(Plans, (SelectedIndex + Plans.Count - 1) % Plans.Count);
        }
    }
}
