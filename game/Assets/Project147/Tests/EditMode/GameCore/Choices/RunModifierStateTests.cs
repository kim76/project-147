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
            Assert.That(state.HasPendingWaveTowerFireRateBoost, Is.False);
            Assert.That(state.HasActiveWaveTowerFireRateBoost, Is.False);
            Assert.That(state.ActiveWaveTowerFireRateMultiplier, Is.EqualTo(1));
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
            Assert.That(result.PendingWaveTowerFireRatePercent, Is.EqualTo(0));
            Assert.That(result.ActiveWaveTowerFireRatePercent, Is.EqualTo(0));
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
            var state = new RunModifierState()
                .AddNextWaveTowerDamagePercent(25)
                .AddNextWaveTowerFireRatePercent(20);

            var result = state.StartWave();

            Assert.That(result.HasPendingWaveTowerDamageBoost, Is.False);
            Assert.That(result.HasActiveWaveTowerDamageBoost, Is.True);
            Assert.That(result.ActiveWaveTowerDamagePercent, Is.EqualTo(25));
            Assert.That(result.ActiveWaveTowerDamageMultiplier, Is.EqualTo(1.25f).Within(0.0001f));
            Assert.That(result.HasPendingWaveTowerFireRateBoost, Is.False);
            Assert.That(result.HasActiveWaveTowerFireRateBoost, Is.True);
            Assert.That(result.ActiveWaveTowerFireRatePercent, Is.EqualTo(20));
            Assert.That(result.ActiveWaveTowerFireRateMultiplier, Is.EqualTo(1.2f).Within(0.0001f));
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
            Assert.That(result.HasActiveWaveTowerFireRateBoost, Is.False);
            Assert.That(result.ActiveWaveTowerFireRateMultiplier, Is.EqualTo(1));
        }

        [Test]
        public void AddNextWaveTowerFireRatePercent_ReturnsStateWithPendingBoost()
        {
            var state = new RunModifierState();

            var result = state.AddNextWaveTowerFireRatePercent(20);

            Assert.That(result.HasPendingWaveTowerFireRateBoost, Is.True);
            Assert.That(result.PendingWaveTowerFireRatePercent, Is.EqualTo(20));
            Assert.That(result.HasActiveWaveTowerFireRateBoost, Is.False);
        }

        [Test]
        public void AddNextWaveTowerFireRatePercent_WhenCalledTwice_StacksPendingBoost()
        {
            var state = new RunModifierState()
                .AddNextWaveTowerFireRatePercent(20)
                .AddNextWaveTowerFireRatePercent(10);

            Assert.That(state.PendingWaveTowerFireRatePercent, Is.EqualTo(30));
        }

        [Test]
        public void AddNextWaveTowerFireRatePercent_WhenPercentIsNegative_Throws()
        {
            var state = new RunModifierState();

            Assert.Throws<ArgumentOutOfRangeException>(() => state.AddNextWaveTowerFireRatePercent(-1));
        }

        [Test]
        public void AddActiveWaveTowerDamagePercent_ReturnsStateWithActiveBoost()
        {
            var state = new RunModifierState();

            var result = state.AddActiveWaveTowerDamagePercent(30);

            Assert.That(result.HasActiveWaveTowerDamageBoost, Is.True);
            Assert.That(result.ActiveWaveTowerDamagePercent, Is.EqualTo(30));
            Assert.That(result.ActiveWaveTowerDamageMultiplier, Is.EqualTo(1.3f).Within(0.0001f));
        }

        [Test]
        public void AddActiveWaveTowerDamagePercent_WhenCalledTwice_StacksActiveBoost()
        {
            var state = new RunModifierState()
                .AddActiveWaveTowerDamagePercent(30)
                .AddActiveWaveTowerDamagePercent(15);

            Assert.That(state.ActiveWaveTowerDamagePercent, Is.EqualTo(45));
        }

        [Test]
        public void AddActiveWaveTowerDamagePercent_WhenPercentIsNegative_Throws()
        {
            var state = new RunModifierState();

            Assert.Throws<ArgumentOutOfRangeException>(() => state.AddActiveWaveTowerDamagePercent(-1));
        }

        [Test]
        public void AddActiveWaveTowerFireRatePercent_ReturnsStateWithActiveBoost()
        {
            var state = new RunModifierState();

            var result = state.AddActiveWaveTowerFireRatePercent(20);

            Assert.That(result.HasActiveWaveTowerFireRateBoost, Is.True);
            Assert.That(result.ActiveWaveTowerFireRatePercent, Is.EqualTo(20));
            Assert.That(result.ActiveWaveTowerFireRateMultiplier, Is.EqualTo(1.2f).Within(0.0001f));
        }

        [Test]
        public void AddActiveWaveTowerFireRatePercent_WhenCalledTwice_StacksActiveBoost()
        {
            var state = new RunModifierState()
                .AddActiveWaveTowerFireRatePercent(20)
                .AddActiveWaveTowerFireRatePercent(10);

            Assert.That(state.ActiveWaveTowerFireRatePercent, Is.EqualTo(30));
        }

        [Test]
        public void AddActiveWaveTowerFireRatePercent_WhenPercentIsNegative_Throws()
        {
            var state = new RunModifierState();

            Assert.Throws<ArgumentOutOfRangeException>(() => state.AddActiveWaveTowerFireRatePercent(-1));
        }
    }
}
