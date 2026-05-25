using System;
using System.Collections.Generic;

namespace Project147.GameCore.Combat
{
    public sealed class TowerUpgradeLoadout
    {
        public TowerUpgradeLoadout(IReadOnlyList<TowerUpgradeDefinition> upgrades)
            : this(upgrades, 0)
        {
        }

        private TowerUpgradeLoadout(IReadOnlyList<TowerUpgradeDefinition> upgrades, int selectedIndex)
        {
            if (upgrades == null)
            {
                throw new ArgumentNullException(nameof(upgrades));
            }

            if (upgrades.Count == 0)
            {
                throw new ArgumentException("Tower upgrade loadout requires at least one upgrade.", nameof(upgrades));
            }

            for (var index = 0; index < upgrades.Count; index++)
            {
                if (upgrades[index] == null)
                {
                    throw new ArgumentNullException(nameof(upgrades), "Tower upgrade loadout cannot contain null upgrades.");
                }
            }

            if (selectedIndex < 0 || selectedIndex >= upgrades.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(selectedIndex), "Selected upgrade index is outside the loadout.");
            }

            Upgrades = new List<TowerUpgradeDefinition>(upgrades);
            SelectedIndex = selectedIndex;
        }

        public IReadOnlyList<TowerUpgradeDefinition> Upgrades { get; }

        public int SelectedIndex { get; }

        public TowerUpgradeDefinition SelectedUpgrade
        {
            get { return Upgrades[SelectedIndex]; }
        }

        public TowerUpgradeLoadout Select(int index)
        {
            return new TowerUpgradeLoadout(Upgrades, index);
        }

        public TowerUpgradeLoadout SelectNext()
        {
            return Select((SelectedIndex + 1) % Upgrades.Count);
        }

        public TowerUpgradeLoadout SelectPrevious()
        {
            return Select((SelectedIndex + Upgrades.Count - 1) % Upgrades.Count);
        }
    }
}
