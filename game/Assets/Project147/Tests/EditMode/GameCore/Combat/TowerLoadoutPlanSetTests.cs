using System;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class TowerLoadoutPlanSetTests
    {
        [Test]
        public void Constructor_WhenPlansAreValid_SelectsFirstPlan()
        {
            var balanced = CreatePlan("balanced");
            var status = CreatePlan("status");

            var planSet = new TowerLoadoutPlanSet(new[] { balanced, status });

            Assert.That(planSet.SelectedIndex, Is.EqualTo(0));
            Assert.That(planSet.SelectedPlan, Is.SameAs(balanced));
        }

        [Test]
        public void Constructor_WhenPlansAreNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new TowerLoadoutPlanSet(null));
        }

        [Test]
        public void Constructor_WhenPlansAreEmpty_Throws()
        {
            Assert.Throws<ArgumentException>(() => new TowerLoadoutPlanSet(Array.Empty<TowerLoadoutPlan>()));
        }

        [Test]
        public void Constructor_WhenPlansContainDuplicateIds_Throws()
        {
            Assert.Throws<ArgumentException>(() => new TowerLoadoutPlanSet(
                new[] { CreatePlan("balanced"), CreatePlan("balanced") }));
        }

        [Test]
        public void SelectNext_WhenAtEnd_WrapsToFirstPlan()
        {
            var planSet = new TowerLoadoutPlanSet(new[] { CreatePlan("balanced"), CreatePlan("status") })
                .SelectNext();

            var result = planSet.SelectNext();

            Assert.That(result.SelectedIndex, Is.EqualTo(0));
        }

        [Test]
        public void SelectPrevious_WhenAtStart_WrapsToLastPlan()
        {
            var planSet = new TowerLoadoutPlanSet(new[] { CreatePlan("balanced"), CreatePlan("status") });

            var result = planSet.SelectPrevious();

            Assert.That(result.SelectedIndex, Is.EqualTo(1));
        }

        private static TowerLoadoutPlan CreatePlan(string id)
        {
            return new TowerLoadoutPlan(
                id,
                new[] { new TowerDefinition($"{id}-tower", 50, 2, 1, 10, DamageType.Kinetic, TowerTargetingMode.First) });
        }
    }
}
