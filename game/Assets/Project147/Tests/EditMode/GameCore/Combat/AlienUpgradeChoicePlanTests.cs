using System;
using System.Collections.Generic;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class AlienUpgradeChoicePlanTests
    {
        [Test]
        public void Constructor_WhenChoicesAreWithinBudget_CalculatesTotals()
        {
            var plan = new AlienUpgradeChoicePlan(
                10,
                new[]
                {
                    CreateChoice("health", 3, 1.25f, 1),
                    CreateChoice("speed", 2, 1, 1.15f)
                });

            Assert.That(plan.Budget, Is.EqualTo(10));
            Assert.That(plan.TotalCost, Is.EqualTo(5));
            Assert.That(plan.RemainingBudget, Is.EqualTo(5));
            Assert.That(plan.IsWithinBudget, Is.True);
        }

        [Test]
        public void Constructor_WhenChoicesExceedBudget_AllowsInvalidPlanningState()
        {
            var plan = new AlienUpgradeChoicePlan(2, new[] { CreateChoice("health", 3, 1.25f, 1) });

            Assert.That(plan.TotalCost, Is.EqualTo(3));
            Assert.That(plan.RemainingBudget, Is.EqualTo(-1));
            Assert.That(plan.IsWithinBudget, Is.False);
        }

        [Test]
        public void Constructor_WhenBudgetIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AlienUpgradeChoicePlan(
                -1,
                Array.Empty<AlienUpgradeChoiceDefinition>()));
        }

        [Test]
        public void Constructor_WhenChoicesAreNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new AlienUpgradeChoicePlan(10, null));
        }

        [Test]
        public void Constructor_WhenChoicesContainNull_Throws()
        {
            Assert.Throws<ArgumentException>(() => new AlienUpgradeChoicePlan(
                10,
                new AlienUpgradeChoiceDefinition[] { null }));
        }

        [Test]
        public void Add_ReturnsNewPlanWithChoice()
        {
            var plan = new AlienUpgradeChoicePlan(10, new[] { CreateChoice("health", 3, 1.25f, 1) });

            var result = plan.Add(CreateChoice("speed", 2, 1, 1.15f));

            Assert.That(result.Choices, Has.Count.EqualTo(2));
            Assert.That(result.TotalCost, Is.EqualTo(5));
            Assert.That(plan.Choices, Has.Count.EqualTo(1));
        }

        [Test]
        public void ApplyTo_AppliesChoicesInOrder()
        {
            var plan = new AlienUpgradeChoicePlan(
                10,
                new[]
                {
                    CreateChoice("health", 3, 1.25f, 1),
                    CreateChoice("speed", 2, 1, 1.15f)
                });
            var alien = new AlienDefinition("debug-basic", 100, 1, 10, new Dictionary<DamageType, float>());

            var result = plan.ApplyTo(alien);

            Assert.That(result.MaxHealth, Is.EqualTo(125));
            Assert.That(result.SpeedCellsPerSecond, Is.EqualTo(1.15f).Within(0.0001f));
        }

        [Test]
        public void ApplyTo_WhenAlienIsNull_Throws()
        {
            var plan = new AlienUpgradeChoicePlan(10, Array.Empty<AlienUpgradeChoiceDefinition>());

            Assert.Throws<ArgumentNullException>(() => plan.ApplyTo(null));
        }

        private static AlienUpgradeChoiceDefinition CreateChoice(
            string id,
            int cost,
            float healthMultiplier,
            float speedMultiplier)
        {
            return new AlienUpgradeChoiceDefinition(
                id,
                id,
                cost,
                new AlienUpgradeDefinition(
                    $"{id}-upgrade",
                    healthMultiplier,
                    speedMultiplier,
                    1,
                    0,
                    DamageType.Kinetic,
                    0));
        }
    }
}
