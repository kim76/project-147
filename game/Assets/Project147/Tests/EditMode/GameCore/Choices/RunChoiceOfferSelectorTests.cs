using System;
using NUnit.Framework;
using Project147.GameCore.Choices;

namespace Project147.Tests.EditMode.GameCore.Choices
{
    public sealed class RunChoiceOfferSelectorTests
    {
        [Test]
        public void Constructor_WhenIndexPickerIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new RunChoiceOfferSelector(null));
        }

        [Test]
        public void SelectOffer_ReturnsRequestedNumberOfUniqueChoices()
        {
            var selector = new RunChoiceOfferSelector(new SequenceChoiceIndexPicker(3, 0, 0));

            var offer = selector.SelectOffer(CreateChoices(), 3);

            Assert.That(offer.Count, Is.EqualTo(3));
            Assert.That(offer[0].Id, Is.EqualTo("damage-boost"));
            Assert.That(offer[1].Id, Is.EqualTo("salvage-drop"));
            Assert.That(offer[2].Id, Is.EqualTo("field-repair"));
        }

        [Test]
        public void SelectOffer_WhenChoicesAreNull_Throws()
        {
            var selector = new RunChoiceOfferSelector(new SequenceChoiceIndexPicker(0));

            Assert.Throws<ArgumentNullException>(() => selector.SelectOffer(null, 3));
        }

        [Test]
        public void SelectOffer_WhenOfferSizeIsZero_Throws()
        {
            var selector = new RunChoiceOfferSelector(new SequenceChoiceIndexPicker(0));

            Assert.Throws<ArgumentOutOfRangeException>(() => selector.SelectOffer(CreateChoices(), 0));
        }

        [Test]
        public void SelectOffer_WhenChoicePoolIsTooSmall_Throws()
        {
            var selector = new RunChoiceOfferSelector(new SequenceChoiceIndexPicker(0));

            Assert.Throws<ArgumentException>(() => selector.SelectOffer(CreateChoices(), 10));
        }

        [Test]
        public void SelectOffer_WhenChoicePoolContainsNull_Throws()
        {
            var selector = new RunChoiceOfferSelector(new SequenceChoiceIndexPicker(0));

            Assert.Throws<ArgumentException>(() => selector.SelectOffer(
                new RunChoiceDefinition[]
                {
                    CreateChoice("salvage-drop"),
                    null,
                    CreateChoice("field-repair")
                },
                2));
        }

        private static RunChoiceDefinition[] CreateChoices()
        {
            return new[]
            {
                CreateChoice("salvage-drop"),
                CreateChoice("field-repair"),
                CreateChoice("scrap-windfall"),
                CreateChoice("damage-boost")
            };
        }

        private static RunChoiceDefinition CreateChoice(string id)
        {
            return new RunChoiceDefinition(id, id, RunChoiceEffectType.AddScrap, 1);
        }

        private sealed class SequenceChoiceIndexPicker : IChoiceIndexPicker
        {
            private readonly int[] indexes;
            private int nextIndex;

            public SequenceChoiceIndexPicker(params int[] indexes)
            {
                this.indexes = indexes;
            }

            public int NextIndex(int exclusiveUpperBound)
            {
                var index = indexes[nextIndex];
                nextIndex++;
                return index;
            }
        }
    }
}
