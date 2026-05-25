using System;
using System.Collections.Generic;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class AlienStateTests
    {
        [Test]
        public void Constructor_WhenDefinitionIsValid_StartsAtMaxHealth()
        {
            var definition = CreateAlien(maxHealth: 100);

            var state = new AlienState(definition);

            Assert.That(state.Definition, Is.SameAs(definition));
            Assert.That(state.Level, Is.EqualTo(1));
            Assert.That(state.CurrentHealth, Is.EqualTo(100));
            Assert.That(state.IsAlive, Is.True);
        }

        [Test]
        public void Constructor_WhenDefinitionIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new AlienState(null));
        }

        [Test]
        public void ApplyDamage_ReducesHealthAndReturnsNewState()
        {
            var state = new AlienState(CreateAlien(maxHealth: 100));

            var result = state.ApplyDamage(35);

            Assert.That(result.CurrentHealth, Is.EqualTo(65));
            Assert.That(state.CurrentHealth, Is.EqualTo(100));
        }

        [Test]
        public void ApplyDamage_WhenDamageExceedsHealth_ClampsHealthAtZero()
        {
            var state = new AlienState(CreateAlien(maxHealth: 100));

            var result = state.ApplyDamage(150);

            Assert.That(result.CurrentHealth, Is.EqualTo(0));
            Assert.That(result.IsAlive, Is.False);
        }

        [Test]
        public void ApplyDamage_WhenDamageIsNegative_Throws()
        {
            var state = new AlienState(CreateAlien(maxHealth: 100));

            Assert.Throws<ArgumentOutOfRangeException>(() => state.ApplyDamage(-1));
        }

        [Test]
        public void Heal_IncreasesHealthAndReturnsNewState()
        {
            var damaged = new AlienState(CreateAlien(maxHealth: 100)).ApplyDamage(40);

            var result = damaged.Heal(25);

            Assert.That(result.CurrentHealth, Is.EqualTo(85));
            Assert.That(damaged.CurrentHealth, Is.EqualTo(60));
        }

        [Test]
        public void Heal_WhenHealExceedsMaxHealth_ClampsAtMaxHealth()
        {
            var damaged = new AlienState(CreateAlien(maxHealth: 100)).ApplyDamage(10);

            var result = damaged.Heal(25);
            Assert.That(result.CurrentHealth, Is.EqualTo(100));
        }

        [Test]
        public void Heal_WhenAmountIsNegative_Throws()
        {
            var state = new AlienState(CreateAlien(maxHealth: 100));

            Assert.Throws<ArgumentOutOfRangeException>(() => state.Heal(-1));
        }

        [Test]
        public void Upgrade_WhenUpgradeDefinitionIsNull_Throws()
        {
            var state = new AlienState(CreateAlien(maxHealth: 100));

            Assert.Throws<ArgumentNullException>(() => state.Upgrade(null));
        }

        [Test]
        public void Upgrade_AppliesUpgradeAndIncrementsLevel()
        {
            var state = new AlienState(CreateAlien(maxHealth: 100)).ApplyDamage(25);
            var upgrade = new AlienUpgradeDefinition("runner-evasion-1", 1.5f, 1.1f, 1.2f, 0.1f, DamageType.Kinetic, 0);

            var result = state.Upgrade(upgrade);

            Assert.That(result.Level, Is.EqualTo(2));
            Assert.That(result.Definition.MaxHealth, Is.EqualTo(150));
            Assert.That(result.CurrentHealth, Is.EqualTo(125));
        }

        [Test]
        public void Upgrade_WhenAlienIsDead_DoesNotReviveAlien()
        {
            var state = new AlienState(CreateAlien(maxHealth: 100)).ApplyDamage(100);
            var upgrade = new AlienUpgradeDefinition("runner-evasion-1", 1.5f, 1.1f, 1.2f, 0.1f, DamageType.Kinetic, 0);

            var result = state.Upgrade(upgrade);

            Assert.That(result.CurrentHealth, Is.EqualTo(0));
            Assert.That(result.IsAlive, Is.False);
        }

        private static AlienDefinition CreateAlien(float maxHealth)
        {
            return new AlienDefinition(
                "runner-basic",
                maxHealth,
                1,
                5,
                new Dictionary<DamageType, float>());
        }
    }
}
