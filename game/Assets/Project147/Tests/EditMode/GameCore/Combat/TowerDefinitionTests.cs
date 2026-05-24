using System;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class TowerDefinitionTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_StoresValues()
        {
            var tower = new TowerDefinition(
                "railgun-basic",
                100,
                3.5f,
                1.25f,
                12,
                DamageType.Kinetic,
                TowerTargetingMode.First);

            Assert.That(tower.Id, Is.EqualTo("railgun-basic"));
            Assert.That(tower.Cost, Is.EqualTo(100));
            Assert.That(tower.Range, Is.EqualTo(3.5f));
            Assert.That(tower.FireRatePerSecond, Is.EqualTo(1.25f));
            Assert.That(tower.Damage, Is.EqualTo(12));
            Assert.That(tower.DamageType, Is.EqualTo(DamageType.Kinetic));
            Assert.That(tower.DefaultTargetingMode, Is.EqualTo(TowerTargetingMode.First));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WhenIdIsMissing_Throws(string id)
        {
            Assert.Throws<ArgumentException>(() => new TowerDefinition(
                id,
                100,
                3,
                1,
                10,
                DamageType.Kinetic,
                TowerTargetingMode.First));
        }

        [Test]
        public void Constructor_WhenCostIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TowerDefinition(
                "railgun-basic",
                -1,
                3,
                1,
                10,
                DamageType.Kinetic,
                TowerTargetingMode.First));
        }

        [Test]
        public void Constructor_WhenRangeIsZero_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TowerDefinition(
                "railgun-basic",
                100,
                0,
                1,
                10,
                DamageType.Kinetic,
                TowerTargetingMode.First));
        }

        [Test]
        public void Constructor_WhenFireRateIsZero_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TowerDefinition(
                "railgun-basic",
                100,
                3,
                0,
                10,
                DamageType.Kinetic,
                TowerTargetingMode.First));
        }

        [Test]
        public void Constructor_WhenDamageIsZero_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TowerDefinition(
                "railgun-basic",
                100,
                3,
                1,
                0,
                DamageType.Kinetic,
                TowerTargetingMode.First));
        }
    }
}

