using System;
using NUnit.Framework;
using Project147.GameCore.Abilities;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Abilities
{
    public sealed class PlayerAbilityStateTests
    {
        [Test]
        public void Constructor_WhenDefinitionIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new PlayerAbilityState(null));
        }

        [Test]
        public void Constructor_WhenDefinitionIsValid_StartsReady()
        {
            var state = new PlayerAbilityState(CreateAbility());

            Assert.That(state.CanActivate, Is.True);
            Assert.That(state.RemainingCooldownSeconds, Is.EqualTo(0));
        }

        [Test]
        public void Activate_WhenReady_StartsCooldown()
        {
            var state = new PlayerAbilityState(CreateAbility());

            var result = state.Activate();

            Assert.That(result.CanActivate, Is.False);
            Assert.That(result.RemainingCooldownSeconds, Is.EqualTo(12));
        }

        [Test]
        public void Activate_WhenOnCooldown_Throws()
        {
            var state = new PlayerAbilityState(CreateAbility()).Activate();

            Assert.Throws<InvalidOperationException>(() => state.Activate());
        }

        [Test]
        public void Tick_ReducesCooldownAndReturnsNewState()
        {
            var state = new PlayerAbilityState(CreateAbility()).Activate();

            var result = state.Tick(5);

            Assert.That(result.RemainingCooldownSeconds, Is.EqualTo(7));
            Assert.That(state.RemainingCooldownSeconds, Is.EqualTo(12));
        }

        [Test]
        public void Tick_WhenDeltaExceedsCooldown_ClampsCooldownAtZero()
        {
            var state = new PlayerAbilityState(CreateAbility()).Activate();

            var result = state.Tick(20);

            Assert.That(result.CanActivate, Is.True);
            Assert.That(result.RemainingCooldownSeconds, Is.EqualTo(0));
        }

        [Test]
        public void Tick_WhenDeltaIsNegative_Throws()
        {
            var state = new PlayerAbilityState(CreateAbility());

            Assert.Throws<ArgumentOutOfRangeException>(() => state.Tick(-0.01f));
        }

        private static PlayerAbilityDefinition CreateAbility()
        {
            return new PlayerAbilityDefinition(
                "freeze-pulse",
                12,
                new AlienStatusEffectDefinition(
                    "freeze-pulse-slow",
                    AlienStatusEffectType.Slow,
                    2,
                    0.35f));
        }
    }
}
