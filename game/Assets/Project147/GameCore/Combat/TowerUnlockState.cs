using System;
using System.Collections.Generic;

namespace Project147.GameCore.Combat
{
    public sealed class TowerUnlockState
    {
        private readonly HashSet<string> unlockedTowerIds;

        public TowerUnlockState(IReadOnlyCollection<string> unlockedTowerIds)
        {
            if (unlockedTowerIds == null)
            {
                throw new ArgumentNullException(nameof(unlockedTowerIds));
            }

            var ids = new HashSet<string>();

            foreach (var id in unlockedTowerIds)
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new ArgumentException("Unlocked tower ids cannot be empty.", nameof(unlockedTowerIds));
                }

                ids.Add(id);
            }

            this.unlockedTowerIds = ids;
            UnlockedTowerIds = ids;
        }

        public IReadOnlyCollection<string> UnlockedTowerIds { get; }

        public bool IsUnlocked(string towerId)
        {
            if (string.IsNullOrWhiteSpace(towerId))
            {
                throw new ArgumentException("Tower id is required.", nameof(towerId));
            }

            return unlockedTowerIds.Contains(towerId);
        }

        public TowerUnlockState Unlock(string towerId)
        {
            if (string.IsNullOrWhiteSpace(towerId))
            {
                throw new ArgumentException("Tower id is required.", nameof(towerId));
            }

            var ids = new HashSet<string>(UnlockedTowerIds)
            {
                towerId
            };

            return new TowerUnlockState(ids);
        }

        public TowerLoadoutPlan FilterPlan(TowerLoadoutPlan plan)
        {
            if (plan == null)
            {
                throw new ArgumentNullException(nameof(plan));
            }

            var towers = new List<TowerDefinition>();

            foreach (var tower in plan.Towers)
            {
                if (IsUnlocked(tower.Id))
                {
                    towers.Add(tower);
                }
            }

            return new TowerLoadoutPlan(plan.Id, towers);
        }
    }
}
