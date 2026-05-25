using System;
using System.Collections.Generic;

namespace Project147.GameCore.Combat
{
    public sealed class TowerLoadout
    {
        public TowerLoadout(IReadOnlyList<TowerDefinition> towers)
            : this(towers, 0)
        {
        }

        private TowerLoadout(IReadOnlyList<TowerDefinition> towers, int selectedIndex)
        {
            if (towers == null)
            {
                throw new ArgumentNullException(nameof(towers));
            }

            if (towers.Count == 0)
            {
                throw new ArgumentException("Tower loadout requires at least one tower.", nameof(towers));
            }

            for (var index = 0; index < towers.Count; index++)
            {
                if (towers[index] == null)
                {
                    throw new ArgumentNullException(nameof(towers), "Tower loadout cannot contain null towers.");
                }
            }

            if (selectedIndex < 0 || selectedIndex >= towers.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(selectedIndex), "Selected tower index is outside the loadout.");
            }

            Towers = new List<TowerDefinition>(towers);
            SelectedIndex = selectedIndex;
        }

        public IReadOnlyList<TowerDefinition> Towers { get; }

        public int SelectedIndex { get; }

        public TowerDefinition SelectedTower
        {
            get { return Towers[SelectedIndex]; }
        }

        public TowerLoadout Select(int index)
        {
            return new TowerLoadout(Towers, index);
        }

        public TowerLoadout SelectNext()
        {
            return Select((SelectedIndex + 1) % Towers.Count);
        }

        public TowerLoadout SelectPrevious()
        {
            return Select((SelectedIndex + Towers.Count - 1) % Towers.Count);
        }
    }
}
