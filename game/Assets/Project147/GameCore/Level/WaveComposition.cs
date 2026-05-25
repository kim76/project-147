using System;
using System.Collections.Generic;

namespace Project147.GameCore.Level
{
    public sealed class WaveComposition
    {
        public WaveComposition(IReadOnlyList<WaveSpawnGroup> groups)
        {
            if (groups == null)
            {
                throw new ArgumentNullException(nameof(groups));
            }

            if (groups.Count == 0)
            {
                throw new ArgumentException("Wave composition requires at least one spawn group.", nameof(groups));
            }

            Groups = new List<WaveSpawnGroup>(groups);

            var totalCount = 0;

            foreach (var group in groups)
            {
                if (string.IsNullOrWhiteSpace(group.AlienId))
                {
                    throw new ArgumentException("Wave composition groups require alien ids.", nameof(groups));
                }

                if (group.Count <= 0)
                {
                    throw new ArgumentException("Wave composition groups require positive counts.", nameof(groups));
                }

                totalCount += group.Count;
            }

            TotalCount = totalCount;
        }

        public IReadOnlyList<WaveSpawnGroup> Groups { get; }

        public int TotalCount { get; }

        public IReadOnlyList<WaveSpawnEntry> BuildSpawnEntries()
        {
            var entries = new List<WaveSpawnEntry>(TotalCount);
            var remainingCounts = new List<int>();

            foreach (var group in Groups)
            {
                remainingCounts.Add(group.Count);
            }

            while (entries.Count < TotalCount)
            {
                for (var groupIndex = 0; groupIndex < Groups.Count; groupIndex++)
                {
                    if (remainingCounts[groupIndex] <= 0)
                    {
                        continue;
                    }

                    entries.Add(new WaveSpawnEntry(Groups[groupIndex].AlienId));
                    remainingCounts[groupIndex]--;
                }
            }

            return entries;
        }
    }
}
