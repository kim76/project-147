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
            Assert.That(state.CurrentShield, Is.EqualTo(0));
            Assert.That(state.ActiveStatusEffects, Is.Empty);
            Assert.That(state.MovementSpeedMultiplier, Is.EqualTo(1));
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
        public void Constructor_WhenDefinitionHasShield_StartsAtFullShield()
        {
            var state = new AlienState(CreateAlien(maxHealth: 100, shieldCapacity: 35));

            Assert.That(state.CurrentHealth, Is.EqualTo(100));
            Assert.That(state.CurrentShield, Is.EqualTo(35));
        }

        [Test]
        public void ApplyDamage_WhenShielded_ReducesShieldBeforeHealth()
        {
            var state = new AlienState(CreateAlien(maxHealth: 100, shieldCapacity: 35));

            var result = state.ApplyDamage(25);

            Assert.That(result.CurrentShield, Is.EqualTo(10));
            Assert.That(result.CurrentHealth, Is.EqualTo(100));
        }

        [Test]
        public void ApplyDamage_WhenDamageExceedsShield_AppliesOverflowToHealth()
        {
            var state = new AlienState(CreateAlien(maxHealth: 100, shieldCapacity: 35));

            var result = state.ApplyDamage(50);

            Assert.That(result.CurrentShield, Is.EqualTo(0));
            Assert.That(result.CurrentHealth, Is.EqualTo(85));
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
        public void Heal_DoesNotRestoreShield()
        {
            var damaged = new AlienState(CreateAlien(maxHealth: 100, shieldCapacity: 35)).ApplyDamage(50);

            var result = damaged.Heal(10);

            Assert.That(result.CurrentHealth, Is.EqualTo(95));
            Assert.That(result.CurrentShield, Is.EqualTo(0));
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
            var state = new AlienState(CreateAlien(maxHealth: 100, shieldCapacity: 20)).ApplyDamage(25);
            var upgrade = new AlienUpgradeDefinition("runner-evasion-1", 1.5f, 1.1f, 1.2f, 0.1f, DamageType.Kinetic, 0);

            var result = state.Upgrade(upgrade);

            Assert.That(result.Level, Is.EqualTo(2));
            Assert.That(result.Definition.MaxHealth, Is.EqualTo(150));
            Assert.That(result.Definition.ShieldCapacity, Is.EqualTo(20));
            Assert.That(result.CurrentHealth, Is.EqualTo(145));
            Assert.That(result.CurrentShield, Is.EqualTo(0));
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

        [Test]
        public void ApplyStatusEffect_WhenDefinitionIsNull_Throws()
        {
            var state = new AlienState(CreateAlien(maxHealth: 100));

            Assert.Throws<ArgumentNullException>(() => state.ApplyStatusEffect(null));
        }

        [Test]
        public void ApplyStatusEffect_AddsActiveEffectAndUpdatesMovementMultiplier()
        {
            var state = new AlienState(CreateAlien(maxHealth: 100));
            var slow = CreateSlow("frost-slow", 2, 0.6f);

            var result = state.ApplyStatusEffect(slow);

            Assert.That(result.ActiveStatusEffects, Has.Count.EqualTo(1));
            Assert.That(result.MovementSpeedMultiplier, Is.EqualTo(0.6f));
            Assert.That(state.ActiveStatusEffects, Is.Empty);
        }

        [Test]
        public void MovementSpeedMultiplier_WhenMultipleSlowsAreActive_UsesStrongestSlow()
        {
            var state = new AlienState(CreateAlien(maxHealth: 100))
                .ApplyStatusEffect(CreateSlow("light-slow", 2, 0.8f))
                .ApplyStatusEffect(CreateSlow("heavy-slow", 2, 0.5f));

            Assert.That(state.MovementSpeedMultiplier, Is.EqualTo(0.5f));
        }

        [Test]
        public void TickStatusEffects_ReducesDurationsAndRemovesExpiredEffects()
        {
            var state = new AlienState(CreateAlien(maxHealth: 100))
                .ApplyStatusEffect(CreateSlow("short-slow", 0.5f, 0.5f))
                .ApplyStatusEffect(CreateSlow("long-slow", 2, 0.8f));

            var result = state.TickStatusEffects(1);

            Assert.That(result.ActiveStatusEffects, Has.Count.EqualTo(1));
            Assert.That(result.ActiveStatusEffects[0].Definition.Id, Is.EqualTo("long-slow"));
            Assert.That(result.ActiveStatusEffects[0].RemainingSeconds, Is.EqualTo(1));
            Assert.That(result.MovementSpeedMultiplier, Is.EqualTo(0.8f));
        }

        [Test]
        public void TickStatusEffects_WhenDeltaIsNegative_Throws()
        {
            var state = new AlienState(CreateAlien(maxHealth: 100));

            Assert.Throws<ArgumentOutOfRangeException>(() => state.TickStatusEffects(-0.01f));
        }

        private static AlienDefinition CreateAlien(float maxHealth, float shieldCapacity = 0)
        {
            return new AlienDefinition(
                "runner-basic",
                maxHealth,
                1,
                5,
                new Dictionary<DamageType, float>(),
                0,
                shieldCapacity);
        }

        private static AlienStatusEffectDefinition CreateSlow(
            string id,
            float durationSeconds,
            float movementSpeedMultiplier)
        {
            return new AlienStatusEffectDefinition(
                id,
                AlienStatusEffectType.Slow,
                durationSeconds,
                movementSpeedMultiplier);
        }
    }
}
