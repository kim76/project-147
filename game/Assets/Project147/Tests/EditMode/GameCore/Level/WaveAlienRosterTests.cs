using System;
using NUnit.Framework;
using Project147.GameCore.Level;

namespace Project147.Tests.EditMode.GameCore.Level
{
    public sealed class WaveAlienRosterTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_StoresIds()
        {
            var roster = CreateRoster();

            Assert.That(roster.BasicAlienId, Is.EqualTo("basic"));
            Assert.That(roster.FastAlienId, Is.EqualTo("fast"));
            Assert.That(roster.ArmouredAlienId, Is.EqualTo("armoured"));
            Assert.That(roster.ShieldedAlienId, Is.EqualTo("shielded"));
            Assert.That(roster.BurrowerAlienId, Is.EqualTo("burrower"));
            Assert.That(roster.RegeneratorAlienId, Is.EqualTo("regenerator"));
            Assert.That(roster.BossAlienId, Is.EqualTo("boss"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WhenRequiredIdIsMissing_Throws(string id)
        {
            Assert.Throws<ArgumentException>(() => new WaveAlienRoster(
                id,
                "fast",
                "armoured",
                "shielded",
                "burrower",
                "regenerator",
                "boss"));
        }

        private static WaveAlienRoster CreateRoster()
        {
            return new WaveAlienRoster(
                "basic",
                "fast",
                "armoured",
                "shielded",
                "burrower",
                "regenerator",
                "boss");
        }
    }
}
