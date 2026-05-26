using System;
using System.Collections.Generic;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class AlienUpgradeDefinitionTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_StoresValues()
        {
            var upgrade = new AlienUpgradeDefinition(
                "runner-evasion-1",
                1.25f,
                1.1f,
                1.2f,
                0.1f,
                DamageType.Kinetic,
                0.15f);

            Assert.That(upgrade.Id, Is.EqualTo("runner-evasion-1"));
            Assert.That(upgrade.HealthMultiplier, Is.EqualTo(1.25f));
            Assert.That(upgrade.SpeedMultiplier, Is.EqualTo(1.1f));
            Assert.That(upgrade.RewardMultiplier, Is.EqualTo(1.2f));
            Assert.That(upgrade.DodgeChanceBonus, Is.EqualTo(0.1f));
            Assert.That(upgrade.ResistanceDamageType, Is.EqualTo(DamageType.Kinetic));
            Assert.That(upgrade.ResistanceBonus, Is.EqualTo(0.15f));
            Assert.That(upgrade.ShieldCapacityBonus, Is.EqualTo(0));
            Assert.That(upgrade.HealthRegenerationBonus, Is.EqualTo(0));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WhenIdIsMissing_Throws(string id)
        {
            Assert.Throws<ArgumentException>(() => new AlienUpgradeDefinition(
                id,
                1,
                1,
                1,
                0,
                DamageType.Kinetic,
                0));
        }

        [TestCase(0)]
        [TestCase(-0.01f)]
        public void Constructor_WhenHealthMultiplierIsZeroOrLess_Throws(float multiplier)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AlienUpgradeDefinition(
                "runner-evasion-1",
                multiplier,
                1,
                1,
                0,
                DamageType.Kinetic,
                0));
        }

        [TestCase(0)]
        [TestCase(-0.01f)]
        public void Constructor_WhenSpeedMultiplierIsZeroOrLess_Throws(float multiplier)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AlienUpgradeDefinition(
                "runner-evasion-1",
                1,
                multiplier,
                1,
                0,
                DamageType.Kinetic,
                0));
        }

        [TestCase(0)]
        [TestCase(-0.01f)]
        public void Constructor_WhenRewardMultiplierIsZeroOrLess_Throws(float multiplier)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AlienUpgradeDefinition(
                "runner-evasion-1",
                1,
                1,
                multiplier,
                0,
                DamageType.Kinetic,
                0));
        }

        [Test]
        public void Constructor_WhenDodgeChanceBonusIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AlienUpgradeDefinition(
                "runner-evasion-1",
                1,
                1,
                1,
                -0.01f,
                DamageType.Kinetic,
                0));
        }

        [Test]
        public void Constructor_WhenResistanceBonusIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AlienUpgradeDefinition(
                "runner-evasion-1",
                1,
                1,
                1,
                0,
                DamageType.Kinetic,
                -0.01f));
        }

        [Test]
        public void Constructor_WhenShieldCapacityBonusIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AlienUpgradeDefinition(
                "runner-evasion-1",
                1,
                1,
                1,
                0,
                DamageType.Kinetic,
                0,
                -0.01f,
                0));
        }

        [Test]
        public void Constructor_WhenHealthRegenerationBonusIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AlienUpgradeDefinition(
                "runner-evasion-1",
                1,
                1,
                1,
                0,
                DamageType.Kinetic,
                0,
                0,
                -0.01f));
        }

        [Test]
        public void ApplyTo_WhenAlienIsNull_Throws()
        {
            var upgrade = new AlienUpgradeDefinition("runner-evasion-1", 1, 1, 1, 0, DamageType.Kinetic, 0);

            Assert.Throws<ArgumentNullException>(() => upgrade.ApplyTo(null));
        }

        [Test]
        public void ApplyTo_AppliesStatChanges()
        {
            var alien = new AlienDefinition(
                "runner-basic",
                80,
                1.5f,
                10,
                new Dictionary<DamageType, float>
                {
                    { DamageType.Kinetic, 0.2f }
                },
                0.1f,
                20,
                3,
                4);
            var upgrade = new AlienUpgradeDefinition(
                "runner-evasion-1",
                1.25f,
                1.2f,
                1.5f,
                0.15f,
                DamageType.Kinetic,
                0.25f,
                12,
                3);

            var result = upgrade.ApplyTo(alien);

            Assert.That(result.Id, Is.EqualTo("runner-basic"));
            Assert.That(result.MaxHealth, Is.EqualTo(100));
            Assert.That(result.SpeedCellsPerSecond, Is.EqualTo(1.8f).Within(0.0001f));
            Assert.That(result.Reward, Is.EqualTo(15));
            Assert.That(result.DodgeChance, Is.EqualTo(0.25f).Within(0.0001f));
            Assert.That(result.ShieldCapacity, Is.EqualTo(32));
            Assert.That(result.TargetableAfterPathProgress, Is.EqualTo(3));
            Assert.That(result.HealthRegenerationPerSecond, Is.EqualTo(7));
            Assert.That(result.GetResistance(DamageType.Kinetic), Is.EqualTo(0.45f).Within(0.0001f));
        }

        [Test]
        public void ApplyTo_WhenRewardMultiplierCreatesFraction_RoundsToNearestInteger()
        {
            var alien = new AlienDefinition(
                "runner-basic",
                80,
                1.5f,
                11,
                new Dictionary<DamageType, float>());
            var upgrade = new AlienUpgradeDefinition("runner-evasion-1", 1, 1, 1.5f, 0, DamageType.Kinetic, 0);

            var result = upgrade.ApplyTo(alien);

            Assert.That(result.Reward, Is.EqualTo(17));
        }

        [Test]
        public void ApplyTo_WhenDodgeChanceWouldExceedOne_Throws()
        {
            var alien = new AlienDefinition(
                "runner-basic",
                80,
                1.5f,
                10,
                new Dictionary<DamageType, float>(),
                0.95f);
            var upgrade = new AlienUpgradeDefinition("runner-evasion-1", 1, 1, 1, 0.1f, DamageType.Kinetic, 0);

            Assert.Throws<InvalidOperationException>(() => upgrade.ApplyTo(alien));
        }

        [Test]
        public void ApplyTo_WhenResistanceWouldExceedOne_Throws()
        {
            var alien = new AlienDefinition(
                "runner-basic",
                80,
                1.5f,
                10,
                new Dictionary<DamageType, float>
                {
                    { DamageType.Kinetic, 0.95f }
                });
            var upgrade = new AlienUpgradeDefinition("runner-evasion-1", 1, 1, 1, 0, DamageType.Kinetic, 0.1f);

            Assert.Throws<InvalidOperationException>(() => upgrade.ApplyTo(alien));
        }
    }
}
