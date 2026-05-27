using System;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class AlienSquadLoadoutPlanSetTests
    {
        [Test]
        public void Constructor_WhenPlansAreValid_SelectsFirstPlan()
        {
            var swarm = CreatePlan("swarm");
            var heavy = CreatePlan("heavy");

            var planSet = new AlienSquadLoadoutPlanSet(new[] { swarm, heavy });

            Assert.That(planSet.SelectedIndex, Is.EqualTo(0));
            Assert.That(planSet.SelectedPlan, Is.SameAs(swarm));
            Assert.That(planSet.SelectedLoadout, Is.SameAs(swarm.Loadout));
        }

        [Test]
        public void Constructor_WhenPlansAreNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new AlienSquadLoadoutPlanSet(null));
        }

        [Test]
        public void Constructor_WhenPlansAreEmpty_Throws()
        {
            Assert.Throws<ArgumentException>(() => new AlienSquadLoadoutPlanSet(Array.Empty<AlienSquadLoadoutPlan>()));
        }

        [Test]
        public void Constructor_WhenPlansContainDuplicateIds_Throws()
        {
            Assert.Throws<ArgumentException>(() => new AlienSquadLoadoutPlanSet(
                new[] { CreatePlan("swarm"), CreatePlan("swarm") }));
        }

        [Test]
        public void SelectNext_WhenAtEnd_WrapsToFirstPlan()
        {
            var planSet = new AlienSquadLoadoutPlanSet(new[] { CreatePlan("swarm"), CreatePlan("heavy") })
                .SelectNext();

            var result = planSet.SelectNext();

            Assert.That(result.SelectedIndex, Is.EqualTo(0));
        }

        [Test]
        public void SelectPrevious_WhenAtStart_WrapsToLastPlan()
        {
            var planSet = new AlienSquadLoadoutPlanSet(new[] { CreatePlan("swarm"), CreatePlan("heavy") });

            var result = planSet.SelectPrevious();

            Assert.That(result.SelectedIndex, Is.EqualTo(1));
        }

        private static AlienSquadLoadoutPlan CreatePlan(string id)
        {
            return new AlienSquadLoadoutPlan(id, AlienSquadLoadoutPlanTests.CreateLoadout());
        }
    }
}
