using System;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class AlienSquadEntryTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_StoresValues()
        {
            var entry = new AlienSquadEntry("debug-fast", 3, 12);

            Assert.That(entry.AlienId, Is.EqualTo("debug-fast"));
            Assert.That(entry.Count, Is.EqualTo(3));
            Assert.That(entry.CostPerAlien, Is.EqualTo(12));
            Assert.That(entry.TotalCost, Is.EqualTo(36));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WhenAlienIdIsMissing_Throws(string alienId)
        {
            Assert.Throws<ArgumentException>(() => new AlienSquadEntry(alienId, 1, 10));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Constructor_WhenCountIsZeroOrLess_Throws(int count)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AlienSquadEntry("debug-fast", count, 10));
        }

        [Test]
        public void Constructor_WhenCostIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AlienSquadEntry("debug-fast", 1, -1));
        }
    }
}
