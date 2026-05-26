using System;
using System.Collections.Generic;

namespace Project147.GameCore.Combat
{
    public sealed class AlienSpawnOrderPlan
    {
        public AlienSpawnOrderPlan(IReadOnlyList<string> alienIds)
        {
            if (alienIds == null)
            {
                throw new ArgumentNullException(nameof(alienIds));
            }

            if (alienIds.Count == 0)
            {
                throw new ArgumentException("Alien spawn order requires at least one alien.", nameof(alienIds));
            }

            foreach (var alienId in alienIds)
            {
                if (string.IsNullOrWhiteSpace(alienId))
                {
                    throw new ArgumentException("Alien spawn order cannot contain empty alien ids.", nameof(alienIds));
                }
            }

            AlienIds = new List<string>(alienIds);
        }

        public IReadOnlyList<string> AlienIds { get; }

        public int Count
        {
            get { return AlienIds.Count; }
        }

        public AlienSpawnOrderPlan Move(int fromIndex, int toIndex)
        {
            if (fromIndex < 0 || fromIndex >= AlienIds.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(fromIndex), "Source spawn index is outside the plan.");
            }

            if (toIndex < 0 || toIndex >= AlienIds.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(toIndex), "Target spawn index is outside the plan.");
            }

            var alienIds = new List<string>(AlienIds);
            var alienId = alienIds[fromIndex];
            alienIds.RemoveAt(fromIndex);
            alienIds.Insert(toIndex, alienId);
            return new AlienSpawnOrderPlan(alienIds);
        }

        public static AlienSpawnOrderPlan FromLoadout(AlienSquadLoadout loadout)
        {
            if (loadout == null)
            {
                throw new ArgumentNullException(nameof(loadout));
            }

            var alienIds = new List<string>();

            foreach (var entry in loadout.Entries)
            {
                for (var index = 0; index < entry.Count; index++)
                {
                    alienIds.Add(entry.AlienId);
                }
            }

            return new AlienSpawnOrderPlan(alienIds);
        }
    }
}
