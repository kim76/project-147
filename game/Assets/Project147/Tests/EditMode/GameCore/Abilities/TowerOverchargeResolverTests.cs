using System;
using NUnit.Framework;
using Project147.GameCore.Abilities;
using Project147.GameCore.Choices;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Abilities
{
    public sealed class TowerOverchargeResolverTests
    {
        [Test]
        public void Resolve_WhenAbilityHasTowerBoosts_AddsActiveWaveModifiers()
        {
            var resolver = new TowerOverchargeResolver();
            var definition = new PlayerAbilityDefinition("tower-overcharge", 20, 35, 25);
            var modifiers = new RunModifierState()
                .AddNextTowerDiscount(15)
                .AddNextWaveTowerDamagePercent(10);

            var result = resolver.Resolve(definition, modifiers);

            Assert.That(result.NextTowerDiscountAmount, Is.EqualTo(15));
            Assert.That(result.PendingWaveTowerDamagePercent, Is.EqualTo(10));
            Assert.That(result.ActiveWaveTowerDamagePercent, Is.EqualTo(35));
            Assert.That(result.ActiveWaveTowerFireRatePercent, Is.EqualTo(25));
        }

        [Test]
        public void Resolve_WhenAbilityHasDamageOnly_AddsDamageModifier()
        {
            var resolver = new TowerOverchargeResolver();
            var definition = new PlayerAbilityDefinition("damage-overcharge", 20, 35, 0);

            var result = resolver.Resolve(definition, new RunModifierState());

            Assert.That(result.ActiveWaveTowerDamagePercent, Is.EqualTo(35));
            Assert.That(result.ActiveWaveTowerFireRatePercent, Is.EqualTo(0));
        }

        [Test]
        public void Resolve_WhenAbilityHasFireRateOnly_AddsFireRateModifier()
        {
            var resolver = new TowerOverchargeResolver();
            var definition = new PlayerAbilityDefinition("rate-overcharge", 20, 0, 25);

            var result = resolver.Resolve(definition, new RunModifierState());

            Assert.That(result.ActiveWaveTowerDamagePercent, Is.EqualTo(0));
            Assert.That(result.ActiveWaveTowerFireRatePercent, Is.EqualTo(25));
        }

        [Test]
        public void Resolve_WhenDefinitionIsNull_Throws()
        {
            var resolver = new TowerOverchargeResolver();

            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(null, new RunModifierState()));
        }

        [Test]
        public void Resolve_WhenModifiersAreNull_Throws()
        {
            var resolver = new TowerOverchargeResolver();
            var definition = new PlayerAbilityDefinition("tower-overcharge", 20, 35, 25);

            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(definition, null));
        }

        [Test]
        public void Resolve_WhenAbilityDoesNotOvercharge_Throws()
        {
            var resolver = new TowerOverchargeResolver();
            var definition = new PlayerAbilityDefinition("orbital-strike", 18, 35, DamageType.Energy);

            Assert.Throws<ArgumentException>(() => resolver.Resolve(definition, new RunModifierState()));
        }
    }
}
