using System;
using NUnit.Framework;
using Project147.GameCore.Abilities;
using Project147.GameCore.Combat;
using Project147.GameCore.Level;

namespace Project147.Tests.EditMode.GameCore.Abilities
{
    public sealed class BaseShieldBurstResolverTests
    {
        [Test]
        public void Resolve_WhenAbilityIsValid_AddsShieldToBase()
        {
            var resolver = new BaseShieldBurstResolver();
            var ability = new PlayerAbilityDefinition("shield-burst", 20, 2);
            var target = new BaseState(10).ApplyLeakDamage(3);

            var result = resolver.Resolve(ability, target);

            Assert.That(result.CurrentHealth, Is.EqualTo(7));
            Assert.That(result.CurrentShield, Is.EqualTo(2));
            Assert.That(target.CurrentShield, Is.EqualTo(0));
        }

        [Test]
        public void Resolve_WhenAbilityIsNull_Throws()
        {
            var resolver = new BaseShieldBurstResolver();

            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(null, new BaseState(10)));
        }

        [Test]
        public void Resolve_WhenTargetIsNull_Throws()
        {
            var resolver = new BaseShieldBurstResolver();

            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(
                new PlayerAbilityDefinition("shield-burst", 20, 2),
                null));
        }

        [Test]
        public void Resolve_WhenAbilityHasNoShield_Throws()
        {
            var resolver = new BaseShieldBurstResolver();

            Assert.Throws<ArgumentException>(() => resolver.Resolve(
                new PlayerAbilityDefinition("orbital-strike", 18, 35, DamageType.Energy),
                new BaseState(10)));
        }
    }
}
