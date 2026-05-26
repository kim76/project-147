using System;
using System.Collections.Generic;

namespace Project147.GameCore.Combat
{
    public sealed class AlienSquadLoadout
    {
        public AlienSquadLoadout(int budget, IReadOnlyList<AlienSquadEntry> entries)
        {
            if (budget < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(budget), "Alien squad budget cannot be negative.");
            }

            if (entries == null)
            {
                throw new ArgumentNullException(nameof(entries));
            }

            var totalCost = 0;

            foreach (var entry in entries)
            {
                if (string.IsNullOrWhiteSpace(entry.AlienId))
                {
                    throw new ArgumentException("Alien squad entries require alien ids.", nameof(entries));
                }

                if (entry.Count <= 0)
                {
                    throw new ArgumentException("Alien squad entries require positive counts.", nameof(entries));
                }

                if (entry.CostPerAlien < 0)
                {
                    throw new ArgumentException("Alien squad entries cannot have negative costs.", nameof(entries));
                }

                totalCost += entry.TotalCost;
            }

            Budget = budget;
            Entries = new List<AlienSquadEntry>(entries);
            TotalCost = totalCost;
        }

        public int Budget { get; }

        public IReadOnlyList<AlienSquadEntry> Entries { get; }

        public int TotalCost { get; }

        public int RemainingBudget
        {
            get { return Budget - TotalCost; }
        }

        public int TotalAliens
        {
            get
            {
                var total = 0;

                foreach (var entry in Entries)
                {
                    total += entry.Count;
                }

                return total;
            }
        }

        public bool IsWithinBudget
        {
            get { return TotalCost <= Budget; }
        }

        public AlienSquadLoadout Add(AlienSquadEntry entry)
        {
            var entries = new List<AlienSquadEntry>(Entries)
            {
                entry
            };

            return new AlienSquadLoadout(Budget, entries);
        }
    }
}
