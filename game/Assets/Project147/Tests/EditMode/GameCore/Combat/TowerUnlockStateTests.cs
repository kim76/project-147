using System;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class TowerUnlockStateTests
    {
        [Test]
        public void Constructor_WhenIdsAreValid_StoresDistinctUnlockedIds()
        {
            var state = new TowerUnlockState(new[] { "railgun", "railgun", "mortar" });

            Assert.That(state.UnlockedTowerIds.Count, Is.EqualTo(2));
            Assert.That(state.IsUnlocked("railgun"), Is.True);
            Assert.That(state.IsUnlocked("mortar"), Is.True);
            Assert.That(state.IsUnlocked("chemical"), Is.False);
        }

        [Test]
        public void Constructor_WhenIdsAreNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new TowerUnlockState(null));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WhenAnIdIsMissing_Throws(string id)
        {
            Assert.Throws<ArgumentException>(() => new TowerUnlockState(new[] { "railgun", id }));
        }

        [Test]
        public void Unlock_WhenTowerIsLocked_ReturnsStateWithTowerUnlocked()
        {
            var state = new TowerUnlockState(new[] { "railgun" });

            var result = state.Unlock("mortar");

            Assert.That(result.IsUnlocked("mortar"), Is.True);
            Assert.That(state.IsUnlocked("mortar"), Is.False);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Unlock_WhenTowerIdIsMissing_Throws(string id)
        {
            var state = new TowerUnlockState(new[] { "railgun" });

            Assert.Throws<ArgumentException>(() => state.Unlock(id));
        }

        [Test]
        public void FilterPlan_ReturnsOnlyUnlockedTowers()
        {
            var state = new TowerUnlockState(new[] { "railgun", "chemical" });
            var plan = new TowerLoadoutPlan(
                "status",
                new[]
                {
                    CreateTower("railgun"),
                    CreateTower("mortar"),
                    CreateTower("chemical")
                });

            var result = state.FilterPlan(plan);

            Assert.That(result.Id, Is.EqualTo("status"));
            Assert.That(result.Towers.Count, Is.EqualTo(2));
            Assert.That(result.Towers[0].Id, Is.EqualTo("railgun"));
            Assert.That(result.Towers[1].Id, Is.EqualTo("chemical"));
        }

        [Test]
        public void FilterPlan_WhenNoTowersAreUnlocked_Throws()
        {
            var state = new TowerUnlockState(Array.Empty<string>());
            var plan = new TowerLoadoutPlan("status", new[] { CreateTower("railgun") });

            Assert.Throws<ArgumentException>(() => state.FilterPlan(plan));
        }

        private static TowerDefinition CreateTower(string id)
        {
            return new TowerDefinition(id, 50, 2, 1, 10, DamageType.Kinetic, TowerTargetingMode.First);
        }
    }
}
