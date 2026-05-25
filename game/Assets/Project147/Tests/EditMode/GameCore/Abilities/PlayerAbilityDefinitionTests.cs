using System;
using NUnit.Framework;
using Project147.GameCore.Abilities;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Abilities
{
    public sealed class PlayerAbilityDefinitionTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_StoresValues()
        {
            var effect = CreateSlow();

            var definition = new PlayerAbilityDefinition("freeze-pulse", 12, effect);

            Assert.That(definition.Id, Is.EqualTo("freeze-pulse"));
            Assert.That(definition.CooldownSeconds, Is.EqualTo(12));
            Assert.That(definition.StatusEffect, Is.SameAs(effect));
            Assert.That(definition.HasStatusEffect, Is.True);
            Assert.That(definition.HasDamage, Is.False);
        }

        [Test]
        public void Constructor_WhenDamageValuesAreValid_StoresDamageValues()
        {
            var definition = new PlayerAbilityDefinition(
                "orbital-strike",
                18,
                35,
                DamageType.Energy);

            Assert.That(definition.Id, Is.EqualTo("orbital-strike"));
            Assert.That(definition.CooldownSeconds, Is.EqualTo(18));
            Assert.That(definition.DamageAmount, Is.EqualTo(35));
            Assert.That(definition.DamageType, Is.EqualTo(DamageType.Energy));
            Assert.That(definition.HasStatusEffect, Is.False);
            Assert.That(definition.HasDamage, Is.True);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WhenIdIsMissing_Throws(string id)
        {
            Assert.Throws<ArgumentException>(() => new PlayerAbilityDefinition(id, 12, CreateSlow()));
        }

        [Test]
        public void Constructor_WhenCooldownIsZero_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PlayerAbilityDefinition(
                "freeze-pulse",
                0,
                CreateSlow()));
        }

        [Test]
        public void Constructor_WhenStatusEffectIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new PlayerAbilityDefinition("freeze-pulse", 12, null));
        }

        [Test]
        public void Constructor_WhenDamageIsZero_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PlayerAbilityDefinition(
                "orbital-strike",
                18,
                0,
                DamageType.Energy));
        }

        private static AlienStatusEffectDefinition CreateSlow()
        {
            return new AlienStatusEffectDefinition(
                "freeze-pulse-slow",
                AlienStatusEffectType.Slow,
                2,
                0.35f);
        }
    }
}
