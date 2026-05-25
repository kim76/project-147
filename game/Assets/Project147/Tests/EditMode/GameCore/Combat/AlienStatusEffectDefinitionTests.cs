using System;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class AlienStatusEffectDefinitionTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_StoresValues()
        {
            var effect = new AlienStatusEffectDefinition(
                "frost-slow",
                AlienStatusEffectType.Slow,
                2.5f,
                0.6f);

            Assert.That(effect.Id, Is.EqualTo("frost-slow"));
            Assert.That(effect.Type, Is.EqualTo(AlienStatusEffectType.Slow));
            Assert.That(effect.DurationSeconds, Is.EqualTo(2.5f));
            Assert.That(effect.MovementSpeedMultiplier, Is.EqualTo(0.6f));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WhenIdIsMissing_Throws(string id)
        {
            Assert.Throws<ArgumentException>(() => new AlienStatusEffectDefinition(
                id,
                AlienStatusEffectType.Slow,
                2,
                0.6f));
        }

        [Test]
        public void Constructor_WhenDurationIsZero_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AlienStatusEffectDefinition(
                "frost-slow",
                AlienStatusEffectType.Slow,
                0,
                0.6f));
        }

        [TestCase(0)]
        [TestCase(-0.01f)]
        [TestCase(1.01f)]
        public void Constructor_WhenMovementSpeedMultiplierIsOutsideValidRange_Throws(float multiplier)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AlienStatusEffectDefinition(
                "frost-slow",
                AlienStatusEffectType.Slow,
                2,
                multiplier));
        }
    }
}
