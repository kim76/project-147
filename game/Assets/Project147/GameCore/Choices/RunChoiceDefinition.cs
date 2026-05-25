using System;

namespace Project147.GameCore.Choices
{
    public sealed class RunChoiceDefinition
    {
        public RunChoiceDefinition(
            string id,
            string label,
            RunChoiceEffectType effectType,
            int amount)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Run choice id is required.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(label))
            {
                throw new ArgumentException("Run choice label is required.", nameof(label));
            }

            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Run choice amount must be greater than zero.");
            }

            Id = id;
            Label = label;
            EffectType = effectType;
            Amount = amount;
        }

        public string Id { get; }

        public string Label { get; }

        public RunChoiceEffectType EffectType { get; }

        public int Amount { get; }
    }
}
