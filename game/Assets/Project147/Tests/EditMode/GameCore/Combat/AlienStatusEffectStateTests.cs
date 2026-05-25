using System;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class AlienStatusEffectStateTests
    {
        [Test]
        public void Constructor_WhenDefinitionIsValid_StartsWithFullDuration()
        {
            var definition = CreateSlow(durationSeconds: 2);

            var state = new AlienStatusEffectState(definition);

            Assert.That(state.Definition, Is.SameAs(definition));
            Assert.That(state.RemainingSeconds, Is.EqualTo(2));
            Assert.That(state.IsExpired, Is.False);
        }

        [Test]
        public void Constructor_WhenDefinitionIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new AlienStatusEffectState(null));
        }

        [Test]
        public void Tick_ReducesRemainingDuration()
        {
            var state = new AlienStatusEffectState(CreateSlow(durationSeconds: 2));

            var result = state.Tick(0.75f);

            Assert.That(result.RemainingSeconds, Is.EqualTo(1.25f));
        }

        [Test]
        public void Tick_WhenDeltaExceedsDuration_ExpiresAtZero()
        {
            var state = new AlienStatusEffectState(CreateSlow(durationSeconds: 2));

            var result = state.Tick(3);

            Assert.That(result.RemainingSeconds, Is.EqualTo(0));
            Assert.That(result.IsExpired, Is.True);
        }

        [Test]
        public void Tick_WhenDeltaIsNegative_Throws()
        {
            var state = new AlienStatusEffectState(CreateSlow(durationSeconds: 2));

            Assert.Throws<ArgumentOutOfRangeException>(() => state.Tick(-0.01f));
        }

        private static AlienStatusEffectDefinition CreateSlow(float durationSeconds)
        {
            return new AlienStatusEffectDefinition(
                "frost-slow",
                AlienStatusEffectType.Slow,
                durationSeconds,
                0.6f);
        }
    }
}
