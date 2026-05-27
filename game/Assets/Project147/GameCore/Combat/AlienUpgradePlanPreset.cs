using System;

namespace Project147.GameCore.Combat
{
    public sealed class AlienUpgradePlanPreset
    {
        public AlienUpgradePlanPreset(string id, AlienUpgradeChoicePlan plan)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Alien upgrade plan preset id is required.", nameof(id));
            }

            Id = id;
            Plan = plan ?? throw new ArgumentNullException(nameof(plan));
        }

        public string Id { get; }

        public AlienUpgradeChoicePlan Plan { get; }
    }
}
