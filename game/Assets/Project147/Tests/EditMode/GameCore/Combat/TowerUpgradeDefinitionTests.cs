using System;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class TowerUpgradeDefinitionTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_StoresValues()
        {
            var upgrade = new TowerUpgradeDefinition(
                "railgun-damage-1",
                75,
                1.25f,
                1.1f,
                0.5f,
                0.1f,
                0.25f);

            Assert.That(upgrade.Id, Is.EqualTo("railgun-damage-1"));
            Assert.That(upgrade.Cost, Is.EqualTo(75));
            Assert.That(upgrade.DamageMultiplier, Is.EqualTo(1.25f));
            Assert.That(upgrade.FireRateMultiplier, Is.EqualTo(1.1f));
            Assert.That(upgrade.RangeBonus, Is.EqualTo(0.5f));
            Assert.That(upgrade.CriticalChanceBonus, Is.EqualTo(0.1f));
            Assert.That(upgrade.CriticalDamageMultiplierBonus, Is.EqualTo(0.25f));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WhenIdIsMissing_Throws(string id)
        {
            Assert.Throws<ArgumentException>(() => new TowerUpgradeDefinition(id, 75, 1, 1, 0, 0, 0));
        }

        [Test]
        public void Constructor_WhenCostIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TowerUpgradeDefinition(
                "railgun-damage-1",
                -1,
                1,
                1,
                0,
                0,
                0));
        }

        [TestCase(0)]
        [TestCase(-0.01f)]
        public void Constructor_WhenDamageMultiplierIsZeroOrLess_Throws(float damageMultiplier)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TowerUpgradeDefinition(
                "railgun-damage-1",
                75,
                damageMultiplier,
                1,
                0,
                0,
                0));
        }

        [TestCase(0)]
        [TestCase(-0.01f)]
        public void Constructor_WhenFireRateMultiplierIsZeroOrLess_Throws(float fireRateMultiplier)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TowerUpgradeDefinition(
                "railgun-damage-1",
                75,
                1,
                fireRateMultiplier,
                0,
                0,
                0));
        }

        [Test]
        public void Constructor_WhenRangeBonusIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TowerUpgradeDefinition(
                "railgun-damage-1",
                75,
                1,
                1,
                -0.01f,
                0,
                0));
        }

        [Test]
        public void Constructor_WhenCriticalChanceBonusIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TowerUpgradeDefinition(
                "railgun-damage-1",
                75,
                1,
                1,
                0,
                -0.01f,
                0));
        }

        [Test]
        public void Constructor_WhenCriticalDamageMultiplierBonusIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TowerUpgradeDefinition(
                "railgun-damage-1",
                75,
                1,
                1,
                0,
                0,
                -0.01f));
        }

        [Test]
        public void ApplyTo_WhenTowerIsNull_Throws()
        {
            var upgrade = new TowerUpgradeDefinition("railgun-damage-1", 75, 1, 1, 0, 0, 0);

            Assert.Throws<ArgumentNullException>(() => upgrade.ApplyTo(null));
        }

        [Test]
        public void ApplyTo_AppliesStatChanges()
        {
            var tower = new TowerDefinition(
                "railgun-basic",
                100,
                3,
                2,
                20,
                DamageType.Kinetic,
                TowerTargetingMode.First,
                0.2f,
                1.5f,
                1.25f,
                0.45f);
            var upgrade = new TowerUpgradeDefinition("railgun-damage-1", 75, 1.25f, 1.1f, 0.5f, 0.1f, 0.25f);

            var result = upgrade.ApplyTo(tower);

            Assert.That(result.Id, Is.EqualTo("railgun-basic"));
            Assert.That(result.Cost, Is.EqualTo(100));
            Assert.That(result.Range, Is.EqualTo(3.5f));
            Assert.That(result.FireRatePerSecond, Is.EqualTo(2.2f).Within(0.0001f));
            Assert.That(result.Damage, Is.EqualTo(25));
            Assert.That(result.DamageType, Is.EqualTo(DamageType.Kinetic));
            Assert.That(result.DefaultTargetingMode, Is.EqualTo(TowerTargetingMode.First));
            Assert.That(result.CriticalChance, Is.EqualTo(0.3f).Within(0.0001f));
            Assert.That(result.CriticalDamageMultiplier, Is.EqualTo(1.75f));
            Assert.That(result.SplashRadius, Is.EqualTo(1.25f));
            Assert.That(result.SplashDamageMultiplier, Is.EqualTo(0.45f));
        }

        [Test]
        public void ApplyTo_PreservesTowerStatusEffects()
        {
            var slow = new AlienStatusEffectDefinition(
                "frost-slow",
                AlienStatusEffectType.Slow,
                2,
                0.6f);
            var tower = new TowerDefinition(
                "frost-basic",
                100,
                3,
                2,
                20,
                DamageType.Frost,
                TowerTargetingMode.First,
                0,
                1,
                0,
                0,
                new[] { slow });
            var upgrade = new TowerUpgradeDefinition("frost-upgrade-1", 75, 1.25f, 1.1f, 0, 0, 0);

            var result = upgrade.ApplyTo(tower);

            Assert.That(result.StatusEffects, Is.EqualTo(new[] { slow }));
        }

        [Test]
        public void ApplyTo_WhenCriticalChanceWouldExceedOne_Throws()
        {
            var tower = new TowerDefinition(
                "railgun-basic",
                100,
                3,
                2,
                20,
                DamageType.Kinetic,
                TowerTargetingMode.First,
                0.95f,
                1.5f);
            var upgrade = new TowerUpgradeDefinition("railgun-damage-1", 75, 1, 1, 0, 0.1f, 0);

            Assert.Throws<InvalidOperationException>(() => upgrade.ApplyTo(tower));
        }
    }
}
