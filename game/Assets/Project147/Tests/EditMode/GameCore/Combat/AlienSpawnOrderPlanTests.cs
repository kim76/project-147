using System;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class AlienSpawnOrderPlanTests
    {
        [Test]
        public void Constructor_WhenAlienIdsAreValid_StoresOrder()
        {
            var plan = new AlienSpawnOrderPlan(new[] { "debug-basic", "debug-fast" });

            Assert.That(plan.AlienIds, Is.EqualTo(new[] { "debug-basic", "debug-fast" }));
            Assert.That(plan.Count, Is.EqualTo(2));
        }

        [Test]
        public void Constructor_WhenAlienIdsAreNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new AlienSpawnOrderPlan(null));
        }

        [Test]
        public void Constructor_WhenAlienIdsAreEmpty_Throws()
        {
            Assert.Throws<ArgumentException>(() => new AlienSpawnOrderPlan(Array.Empty<string>()));
        }

        [Test]
        public void Constructor_WhenAlienIdsContainEmptyId_Throws()
        {
            Assert.Throws<ArgumentException>(() => new AlienSpawnOrderPlan(new[] { "debug-basic", "" }));
        }

        [Test]
        public void Move_WhenIndexesAreValid_ReturnsReorderedPlan()
        {
            var plan = new AlienSpawnOrderPlan(new[] { "debug-basic", "debug-fast", "debug-boss" });

            var result = plan.Move(2, 0);

            Assert.That(result.AlienIds, Is.EqualTo(new[] { "debug-boss", "debug-basic", "debug-fast" }));
            Assert.That(plan.AlienIds, Is.EqualTo(new[] { "debug-basic", "debug-fast", "debug-boss" }));
        }

        [TestCase(-1, 0)]
        [TestCase(3, 0)]
        [TestCase(0, -1)]
        [TestCase(0, 3)]
        public void Move_WhenIndexIsOutsidePlan_Throws(int fromIndex, int toIndex)
        {
            var plan = new AlienSpawnOrderPlan(new[] { "debug-basic", "debug-fast", "debug-boss" });

            Assert.Throws<ArgumentOutOfRangeException>(() => plan.Move(fromIndex, toIndex));
        }

        [Test]
        public void FromLoadout_ExpandsSquadEntriesInOrder()
        {
            var loadout = new AlienSquadLoadout(
                100,
                new[]
                {
                    new AlienSquadEntry("debug-basic", 2, 10),
                    new AlienSquadEntry("debug-fast", 1, 15)
                });

            var plan = AlienSpawnOrderPlan.FromLoadout(loadout);

            Assert.That(plan.AlienIds, Is.EqualTo(new[] { "debug-basic", "debug-basic", "debug-fast" }));
        }

        [Test]
        public void FromLoadout_WhenLoadoutIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => AlienSpawnOrderPlan.FromLoadout(null));
        }
    }
}
