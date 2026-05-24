using System;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class TowerStateTests
    {
        [Test]
        public void Constructor_WhenDefinitionIsValid_StartsReadyToFire()
        {
            var definition = CreateTower(fireRatePerSecond: 2);

            var state = new TowerState(definition);

            Assert.That(state.Definition, Is.SameAs(definition));
            Assert.That(state.SecondsUntilReady, Is.EqualTo(0));
            Assert.That(state.CanFire, Is.True);
        }

        [Test]
        public void Constructor_WhenDefinitionIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new TowerState(null));
        }

        [Test]
        public void MarkFired_SetsCooldownFromFireRate()
        {
            var state = new TowerState(CreateTower(fireRatePerSecond: 2));

            var result = state.MarkFired();
            Assert.That(result.SecondsUntilReady, Is.EqualTo(0.5f));
            Assert.That(result.CanFire, Is.False);
        }

        [Test]
        public void Tick_ReducesCooldownAndReturnsNewState()
        {
            var state = new TowerState(CreateTower(fireRatePerSecond: 2)).MarkFired();

            var result = state.Tick(0.2f);

            Assert.That(result.SecondsUntilReady, Is.EqualTo(0.3f).Within(0.0001f));
            Assert.That(state.SecondsUntilReady, Is.EqualTo(0.5f));
        }

        [Test]
        public void Tick_WhenDeltaExceedsCooldown_ClampsCooldownAtZero()
        {
            var state = new TowerState(CreateTower(fireRatePerSecond: 2)).MarkFired();

            var result = state.Tick(1);
            Assert.That(result.SecondsUntilReady, Is.EqualTo(0));
            Assert.That(result.CanFire, Is.True);
        }

        [Test]
        public void Tick_WhenDeltaIsNegative_Throws()
        {
            var state = new TowerState(CreateTower(fireRatePerSecond: 2));

            Assert.Throws<ArgumentOutOfRangeException>(() => state.Tick(-1));
        }

        private static TowerDefinition CreateTower(float fireRatePerSecond)
        {
            return new TowerDefinition(
                "railgun-basic",
                100,
                3,
                fireRatePerSecond,
                10,
                DamageType.Kinetic,
                TowerTargetingMode.First);
        }
    }
}

