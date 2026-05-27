using System;

namespace Project147.GameCore.Combat
{
    public sealed class AlienSquadLoadoutPlan
    {
        public AlienSquadLoadoutPlan(string id, AlienSquadLoadout loadout)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Alien squad loadout plan id is required.", nameof(id));
            }

            Id = id;
            Loadout = loadout ?? throw new ArgumentNullException(nameof(loadout));
        }

        public string Id { get; }

        public AlienSquadLoadout Loadout { get; }
    }
}
