using System;
using System.Collections.Generic;
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
            Assert.That(tower.CriticalChance, Is.EqualTo(0));
            Assert.That(tower.CriticalDamageMultiplier, Is.EqualTo(1));
            Assert.That(tower.SplashRadius, Is.EqualTo(0));
            Assert.That(tower.SplashDamageMultiplier, Is.EqualTo(0));
            Assert.That(tower.StatusEffects, Is.Empty);
        }

        [Test]
        public void Constructor_WhenSplashValuesAreValid_StoresValues()
        {
            var tower = new TowerDefinition(
                "mortar-basic",
                100,
                3.5f,
                1.25f,
                12,
                DamageType.Explosive,
                TowerTargetingMode.Strongest,
                0,
                1,
                1.2f,
                0.45f);

            Assert.That(tower.SplashRadius, Is.EqualTo(1.2f));
            Assert.That(tower.SplashDamageMultiplier, Is.EqualTo(0.45f));
        }

        [Test]
        public void Constructor_WhenCriticalValuesAreValid_StoresValues()
        {
            var tower = new TowerDefinition(
                "railgun-basic",
                100,
                3.5f,
                1.25f,
                12,
                DamageType.Kinetic,
                TowerTargetingMode.First,
                0.2f,
                1.75f);

            Assert.That(tower.CriticalChance, Is.EqualTo(0.2f));
            Assert.That(tower.CriticalDamageMultiplier, Is.EqualTo(1.75f));
        }

        [Test]
        public void Constructor_WhenStatusEffectsAreProvided_StoresValues()
        {
            var slow = new AlienStatusEffectDefinition(
                "frost-slow",
                AlienStatusEffectType.Slow,
                2,
                0.6f);

            var tower = new TowerDefinition(
                "frost-basic",
                100,
                3.5f,
                1.25f,
                12,
                DamageType.Frost,
                TowerTargetingMode.First,
                0,
                1,
                0,
                0,
                new[] { slow });

            Assert.That(tower.StatusEffects, Is.EqualTo(new[] { slow }));
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

        [TestCase(-0.01f)]
        [TestCase(1.01f)]
        public void Constructor_WhenCriticalChanceIsOutsideZeroToOne_Throws(float criticalChance)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TowerDefinition(
                "railgun-basic",
                100,
                3,
                1,
                10,
                DamageType.Kinetic,
                TowerTargetingMode.First,
                criticalChance,
                1.5f));
        }

        [TestCase(0)]
        [TestCase(0.99f)]
        public void Constructor_WhenCriticalMultiplierIsLessThanOne_Throws(float criticalDamageMultiplier)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TowerDefinition(
                "railgun-basic",
                100,
                3,
                1,
                10,
                DamageType.Kinetic,
                TowerTargetingMode.First,
                0.2f,
                criticalDamageMultiplier));
        }

        [Test]
        public void Constructor_WhenSplashRadiusIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TowerDefinition(
                "mortar-basic",
                100,
                3,
                1,
                10,
                DamageType.Explosive,
                TowerTargetingMode.Strongest,
                0,
                1,
                -0.01f,
                0.5f));
        }

        [TestCase(-0.01f)]
        [TestCase(1.01f)]
        public void Constructor_WhenSplashDamageMultiplierIsOutsideZeroToOne_Throws(float splashDamageMultiplier)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TowerDefinition(
                "mortar-basic",
                100,
                3,
                1,
                10,
                DamageType.Explosive,
                TowerTargetingMode.Strongest,
                0,
                1,
                1,
                splashDamageMultiplier));
        }

        [Test]
        public void Constructor_WhenStatusEffectsContainNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new TowerDefinition(
                "railgun-basic",
                100,
                3,
                1,
                10,
                DamageType.Kinetic,
                TowerTargetingMode.First,
                0,
                1,
                0,
                0,
                new List<AlienStatusEffectDefinition> { null }));
        }
    }
}
