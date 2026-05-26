using System;
using System.Collections.Generic;

namespace Project147.GameCore.Combat
{
    public sealed class TowerLoadoutPlan
    {
        public TowerLoadoutPlan(string id, IReadOnlyList<TowerDefinition> towers)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Tower loadout plan id is required.", nameof(id));
            }

            if (towers == null)
            {
                throw new ArgumentNullException(nameof(towers));
            }

            if (towers.Count == 0)
            {
                throw new ArgumentException("Tower loadout plan requires at least one tower.", nameof(towers));
            }

            var towerIds = new HashSet<string>();

            foreach (var tower in towers)
            {
                if (tower == null)
                {
                    throw new ArgumentNullException(nameof(towers), "Tower loadout plan cannot contain null towers.");
                }

                if (!towerIds.Add(tower.Id))
                {
                    throw new ArgumentException("Tower loadout plan cannot contain duplicate towers.", nameof(towers));
                }
            }

            Id = id;
            Towers = new List<TowerDefinition>(towers);
        }

        public string Id { get; }

        public IReadOnlyList<TowerDefinition> Towers { get; }

        public TowerLoadout CreateLoadout()
        {
            return new TowerLoadout(Towers);
        }
    }
}
