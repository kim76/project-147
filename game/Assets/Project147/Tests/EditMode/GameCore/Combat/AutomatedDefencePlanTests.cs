using System;
using NUnit.Framework;
using Project147.GameCore.Combat;
using Project147.GameCore.Grid;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class AutomatedDefencePlanTests
    {
        [Test]
        public void Constructor_WhenPlacementsAreValid_CalculatesTotals()
        {
            var tower = AutomatedDefencePlannerTests.CreateTower("railgun", 40, 10, 1);
            var plan = new AutomatedDefencePlan(
                100,
                new[]
                {
                    new AutomatedDefencePlacement(new GridCoordinate(1, 1), tower),
                    new AutomatedDefencePlacement(new GridCoordinate(2, 1), tower)
                });

            Assert.That(plan.Budget, Is.EqualTo(100));
            Assert.That(plan.TotalCost, Is.EqualTo(80));
            Assert.That(plan.RemainingBudget, Is.EqualTo(20));
            Assert.That(plan.IsWithinBudget, Is.True);
        }

        [Test]
        public void Constructor_WhenBudgetIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AutomatedDefencePlan(
                -1,
                Array.Empty<AutomatedDefencePlacement>()));
        }

        [Test]
        public void Constructor_WhenPlacementsAreNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new AutomatedDefencePlan(100, null));
        }

        [Test]
        public void Constructor_WhenPlacementsContainNull_Throws()
        {
            Assert.Throws<ArgumentException>(() => new AutomatedDefencePlan(
                100,
                new AutomatedDefencePlacement[] { null }));
        }
    }
}
