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
            Assert.That(state.HasPendingWaveTowerDamageBoost, Is.False);
            Assert.That(state.HasActiveWaveTowerDamageBoost, Is.False);
            Assert.That(state.ActiveWaveTowerDamageMultiplier, Is.EqualTo(1));
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
            var state = new RunModifierState()
                .AddNextTowerDiscount(30)
                .AddNextWaveTowerDamagePercent(25)
                .StartWave();

            var result = state.ConsumeNextTowerDiscount();

            Assert.That(result.HasNextTowerDiscount, Is.False);
            Assert.That(result.NextTowerDiscountAmount, Is.EqualTo(0));
            Assert.That(result.PendingWaveTowerDamagePercent, Is.EqualTo(0));
            Assert.That(result.ActiveWaveTowerDamagePercent, Is.EqualTo(25));
        }

        [Test]
        public void AddNextWaveTowerDamagePercent_ReturnsStateWithPendingBoost()
        {
            var state = new RunModifierState();

            var result = state.AddNextWaveTowerDamagePercent(25);

            Assert.That(result.HasPendingWaveTowerDamageBoost, Is.True);
            Assert.That(result.PendingWaveTowerDamagePercent, Is.EqualTo(25));
            Assert.That(result.HasActiveWaveTowerDamageBoost, Is.False);
        }

        [Test]
        public void AddNextWaveTowerDamagePercent_WhenCalledTwice_StacksPendingBoost()
        {
            var state = new RunModifierState()
                .AddNextWaveTowerDamagePercent(25)
                .AddNextWaveTowerDamagePercent(10);

            Assert.That(state.PendingWaveTowerDamagePercent, Is.EqualTo(35));
        }

        [Test]
        public void AddNextWaveTowerDamagePercent_WhenPercentIsNegative_Throws()
        {
            var state = new RunModifierState();

            Assert.Throws<ArgumentOutOfRangeException>(() => state.AddNextWaveTowerDamagePercent(-1));
        }

        [Test]
        public void StartWave_MovesPendingDamageBoostToActiveBoost()
        {
            var state = new RunModifierState().AddNextWaveTowerDamagePercent(25);

            var result = state.StartWave();

            Assert.That(result.HasPendingWaveTowerDamageBoost, Is.False);
            Assert.That(result.HasActiveWaveTowerDamageBoost, Is.True);
            Assert.That(result.ActiveWaveTowerDamagePercent, Is.EqualTo(25));
            Assert.That(result.ActiveWaveTowerDamageMultiplier, Is.EqualTo(1.25f).Within(0.0001f));
        }

        [Test]
        public void EndWave_ClearsActiveDamageBoost()
        {
            var state = new RunModifierState()
                .AddNextWaveTowerDamagePercent(25)
                .StartWave();

            var result = state.EndWave();

            Assert.That(result.HasActiveWaveTowerDamageBoost, Is.False);
            Assert.That(result.ActiveWaveTowerDamageMultiplier, Is.EqualTo(1));
        }
    }
}
