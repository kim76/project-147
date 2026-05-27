using System;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class AlienSquadLoadoutPlanTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_StoresValues()
        {
            var loadout = CreateLoadout();

            var plan = new AlienSquadLoadoutPlan("swarm", loadout);

            Assert.That(plan.Id, Is.EqualTo("swarm"));
            Assert.That(plan.Loadout, Is.SameAs(loadout));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WhenIdIsMissing_Throws(string id)
        {
            Assert.Throws<ArgumentException>(() => new AlienSquadLoadoutPlan(id, CreateLoadout()));
        }

        [Test]
        public void Constructor_WhenLoadoutIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new AlienSquadLoadoutPlan("swarm", null));
        }

        public static AlienSquadLoadout CreateLoadout()
        {
            return new AlienSquadLoadout(
                100,
                new[] { new AlienSquadEntry("debug-basic", 3, 10) });
        }
    }
}
