using System;
using NUnit.Framework;
using Project147.GameCore.Choices;

namespace Project147.Tests.EditMode.GameCore.Choices
{
    public sealed class RandomChoiceIndexPickerTests
    {
        [Test]
        public void Constructor_WhenRandomIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new RandomChoiceIndexPicker(null));
        }

        [Test]
        public void NextIndex_WhenUpperBoundIsValid_ReturnsIndexWithinBounds()
        {
            var picker = new RandomChoiceIndexPicker(new Random(1));

            var index = picker.NextIndex(3);

            Assert.That(index, Is.GreaterThanOrEqualTo(0));
            Assert.That(index, Is.LessThan(3));
        }

        [Test]
        public void NextIndex_WhenUpperBoundIsZero_Throws()
        {
            var picker = new RandomChoiceIndexPicker(new Random(1));

            Assert.Throws<ArgumentOutOfRangeException>(() => picker.NextIndex(0));
        }
    }
}
