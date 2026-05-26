using System;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class TowerLoadoutPlanTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_StoresTowers()
        {
            var railgun = CreateTower("railgun");
            var mortar = CreateTower("mortar");

            var plan = new TowerLoadoutPlan("balanced", new[] { railgun, mortar });

            Assert.That(plan.Id, Is.EqualTo("balanced"));
            Assert.That(plan.Towers, Is.EqualTo(new[] { railgun, mortar }));
            Assert.That(plan.CreateLoadout().Towers, Is.EqualTo(new[] { railgun, mortar }));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WhenIdIsMissing_Throws(string id)
        {
            Assert.Throws<ArgumentException>(() => new TowerLoadoutPlan(id, new[] { CreateTower("railgun") }));
        }

        [Test]
        public void Constructor_WhenTowersAreNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new TowerLoadoutPlan("balanced", null));
        }

        [Test]
        public void Constructor_WhenTowersAreEmpty_Throws()
        {
            Assert.Throws<ArgumentException>(() => new TowerLoadoutPlan("balanced", Array.Empty<TowerDefinition>()));
        }

        [Test]
        public void Constructor_WhenTowersContainDuplicateIds_Throws()
        {
            Assert.Throws<ArgumentException>(() => new TowerLoadoutPlan(
                "balanced",
                new[] { CreateTower("railgun"), CreateTower("railgun") }));
        }

        private static TowerDefinition CreateTower(string id)
        {
            return new TowerDefinition(id, 50, 2, 1, 10, DamageType.Kinetic, TowerTargetingMode.First);
        }
    }
}
