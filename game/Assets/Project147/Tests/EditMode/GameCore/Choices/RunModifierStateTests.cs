using System;
using NUnit.Framework;
using Project147.GameCore.Choices;

namespace Project147.Tests.EditMode.GameCore.Choices
{
    public sealed class RunModifierStateTests
    {
        [Test]
        public void Constructor_CreatesEmptyModifiers()
        {
            var state = new RunModifierState();

            Assert.That(state.HasNextTowerDiscount, Is.False);
            Assert.That(state.NextTowerDiscountAmount, Is.EqualTo(0));
        }

        [Test]
        public void AddNextTowerDiscount_ReturnsStateWithAddedDiscount()
        {
            var state = new RunModifierState();

            var result = state.AddNextTowerDiscount(25);

            Assert.That(result.HasNextTowerDiscount, Is.True);
            Assert.That(result.NextTowerDiscountAmount, Is.EqualTo(25));
            Assert.That(state.NextTowerDiscountAmount, Is.EqualTo(0));
        }

        [Test]
        public void AddNextTowerDiscount_WhenCalledTwice_StacksDiscount()
        {
            var state = new RunModifierState()
                .AddNextTowerDiscount(20)
                .AddNextTowerDiscount(15);

            Assert.That(state.NextTowerDiscountAmount, Is.EqualTo(35));
        }

        [Test]
        public void CalculateTowerCost_WhenDiscountIsLessThanCost_ReducesCost()
        {
            var state = new RunModifierState().AddNextTowerDiscount(30);

            Assert.That(state.CalculateTowerCost(80), Is.EqualTo(50));
        }

        [Test]
        public void CalculateTowerCost_WhenDiscountExceedsCost_ClampsAtZero()
        {
            var state = new RunModifierState().AddNextTowerDiscount(120);

            Assert.That(state.CalculateTowerCost(80), Is.EqualTo(0));
        }

        [Test]
        public void CalculateTowerCost_WhenOriginalCostIsNegative_Throws()
        {
            var state = new RunModifierState();

            Assert.Throws<ArgumentOutOfRangeException>(() => state.CalculateTowerCost(-1));
        }

        [Test]
        public void AddNextTowerDiscount_WhenAmountIsNegative_Throws()
        {
            var state = new RunModifierState();

            Assert.Throws<ArgumentOutOfRangeException>(() => state.AddNextTowerDiscount(-1));
        }

        [Test]
        public void ConsumeNextTowerDiscount_ReturnsStateWithoutDiscount()
        {
            var state = new RunModifierState().AddNextTowerDiscount(30);

            var result = state.ConsumeNextTowerDiscount();

            Assert.That(result.HasNextTowerDiscount, Is.False);
            Assert.That(result.NextTowerDiscountAmount, Is.EqualTo(0));
        }
    }
}
