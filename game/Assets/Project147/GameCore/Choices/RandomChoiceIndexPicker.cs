using System;

namespace Project147.GameCore.Choices
{
    public sealed class RandomChoiceIndexPicker : IChoiceIndexPicker
    {
        private readonly Random random;

        public RandomChoiceIndexPicker()
            : this(new Random())
        {
        }

        public RandomChoiceIndexPicker(Random random)
        {
            this.random = random ?? throw new ArgumentNullException(nameof(random));
        }

        public int NextIndex(int exclusiveUpperBound)
        {
            if (exclusiveUpperBound <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(exclusiveUpperBound),
                    "Exclusive upper bound must be greater than zero.");
            }

            return random.Next(exclusiveUpperBound);
        }
    }
}
