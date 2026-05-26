using System;

namespace Project147.GameCore.Combat
{
    public sealed class AlienUpgradeChoiceDefinition
    {
        public AlienUpgradeChoiceDefinition(
            string id,
            string label,
            int cost,
            AlienUpgradeDefinition upgrade)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Alien upgrade choice id is required.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(label))
            {
                throw new ArgumentException("Alien upgrade choice label is required.", nameof(label));
            }

            if (cost < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(cost), "Alien upgrade choice cost cannot be negative.");
            }

            Id = id;
            Label = label;
            Cost = cost;
            Upgrade = upgrade ?? throw new ArgumentNullException(nameof(upgrade));
        }

        public string Id { get; }

        public string Label { get; }

        public int Cost { get; }

        public AlienUpgradeDefinition Upgrade { get; }

        public AlienDefinition ApplyTo(AlienDefinition alien)
        {
            return Upgrade.ApplyTo(alien);
        }
    }
}
