using System;
using System.Collections.Generic;

namespace Project147.GameCore.Choices
{
    public sealed class RunChoiceOfferSelector
    {
        private readonly IChoiceIndexPicker indexPicker;

        public RunChoiceOfferSelector(IChoiceIndexPicker indexPicker)
        {
            this.indexPicker = indexPicker ?? throw new ArgumentNullException(nameof(indexPicker));
        }

        public IReadOnlyList<RunChoiceDefinition> SelectOffer(
            IReadOnlyList<RunChoiceDefinition> choices,
            int offerSize)
        {
            if (choices == null)
            {
                throw new ArgumentNullException(nameof(choices));
            }

            if (offerSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offerSize), "Offer size must be greater than zero.");
            }

            if (choices.Count < offerSize)
            {
                throw new ArgumentException("There must be at least as many choices as requested offers.", nameof(choices));
            }

            var pool = new List<RunChoiceDefinition>(choices.Count);

            foreach (var choice in choices)
            {
                if (choice == null)
                {
                    throw new ArgumentException("Choice pool cannot contain null choices.", nameof(choices));
                }

                pool.Add(choice);
            }

            var offer = new List<RunChoiceDefinition>(offerSize);

            while (offer.Count < offerSize)
            {
                var selectedIndex = indexPicker.NextIndex(pool.Count);
                offer.Add(pool[selectedIndex]);
                pool.RemoveAt(selectedIndex);
            }

            return offer;
        }
    }
}
