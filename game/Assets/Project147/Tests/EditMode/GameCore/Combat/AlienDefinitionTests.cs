using System;
using System.Collections.Generic;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class AlienDefinitionTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_StoresValues()
        {
            var alien = new AlienDefinition(
                "runner-basic",
                50,
                1.75f,
                5,
                new Dictionary<DamageType, float>
                {
                    { DamageType.Energy, 0.25f }
                });

            Assert.That(alien.Id, Is.EqualTo("runner-basic"));
            Assert.That(alien.MaxHealth, Is.EqualTo(50));
            Assert.That(alien.SpeedCellsPerSecond, Is.EqualTo(1.75f));
            Assert.That(alien.Reward, Is.EqualTo(5));
            Assert.That(alien.GetResistance(DamageType.Energy), Is.EqualTo(0.25f));
            Assert.That(alien.GetResistance(DamageType.Kinetic), Is.EqualTo(0));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WhenIdIsMissing_Throws(string id)
        {
            Assert.Throws<ArgumentException>(() => new AlienDefinition(
                id,
                50,
                1,
                5,
                new Dictionary<DamageType, float>()));
        }

        [Test]
        public void Constructor_WhenHealthIsZero_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AlienDefinition(
                "runner-basic",
                0,
                1,
                5,
                new Dictionary<DamageType, float>()));
        }

        [Test]
        public void Constructor_WhenSpeedIsZero_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AlienDefinition(
                "runner-basic",
                50,
                0,
                5,
                new Dictionary<DamageType, float>()));
        }

        [Test]
        public void Constructor_WhenRewardIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AlienDefinition(
                "runner-basic",
                50,
                1,
                -1,
                new Dictionary<DamageType, float>()));
        }

        [Test]
        public void Constructor_WhenResistancesAreNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new AlienDefinition(
                "runner-basic",
                50,
                1,
                5,
                null));
        }

        [TestCase(-0.01f)]
        [TestCase(1.01f)]
        public void Constructor_WhenResistanceIsOutsideZeroToOne_Throws(float resistance)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AlienDefinition(
                "runner-basic",
                50,
                1,
                5,
                new Dictionary<DamageType, float>
                {
                    { DamageType.Kinetic, resistance }
                }));
        }
    }
}

