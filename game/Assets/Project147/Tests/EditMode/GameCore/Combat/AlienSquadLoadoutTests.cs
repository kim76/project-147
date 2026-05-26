using System;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class AlienSquadLoadoutTests
    {
        [Test]
        public void Constructor_WhenEntriesAreWithinBudget_CalculatesTotals()
        {
            var loadout = new AlienSquadLoadout(
                100,
                new[]
                {
                    new AlienSquadEntry("debug-basic", 3, 10),
                    new AlienSquadEntry("debug-fast", 2, 15)
                });

            Assert.That(loadout.Budget, Is.EqualTo(100));
            Assert.That(loadout.TotalCost, Is.EqualTo(60));
            Assert.That(loadout.RemainingBudget, Is.EqualTo(40));
            Assert.That(loadout.TotalAliens, Is.EqualTo(5));
            Assert.That(loadout.IsWithinBudget, Is.True);
        }

        [Test]
        public void Constructor_WhenEntriesExceedBudget_AllowsInvalidPlanningState()
        {
            var loadout = new AlienSquadLoadout(
                30,
                new[] { new AlienSquadEntry("debug-basic", 4, 10) });

            Assert.That(loadout.TotalCost, Is.EqualTo(40));
            Assert.That(loadout.RemainingBudget, Is.EqualTo(-10));
            Assert.That(loadout.IsWithinBudget, Is.False);
        }

        [Test]
        public void Constructor_WhenBudgetIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AlienSquadLoadout(
                -1,
                Array.Empty<AlienSquadEntry>()));
        }

        [Test]
        public void Constructor_WhenEntriesAreNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new AlienSquadLoadout(100, null));
        }

        [Test]
        public void Constructor_WhenEntriesContainDefaultEntry_Throws()
        {
            Assert.Throws<ArgumentException>(() => new AlienSquadLoadout(
                100,
                new[] { default(AlienSquadEntry) }));
        }

        [Test]
        public void Add_ReturnsNewLoadoutWithEntry()
        {
            var loadout = new AlienSquadLoadout(
                100,
                new[] { new AlienSquadEntry("debug-basic", 3, 10) });

            var result = loadout.Add(new AlienSquadEntry("debug-fast", 2, 15));

            Assert.That(result.Entries, Has.Count.EqualTo(2));
            Assert.That(result.TotalCost, Is.EqualTo(60));
            Assert.That(loadout.Entries, Has.Count.EqualTo(1));
        }
    }
}
