using System;
using System.Collections.Generic;
using NUnit.Framework;
using Project147.GameCore.Abilities;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Abilities
{
    public sealed class FreezePulseResolverTests
    {
        [Test]
        public void Resolve_WhenAbilityIsNull_Throws()
        {
            var resolver = new FreezePulseResolver();

            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(null, new[] { CreateAlienState() }));
        }

        [Test]
        public void Resolve_WhenTargetsAreNull_Throws()
        {
            var resolver = new FreezePulseResolver();

            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(CreateAbility(), null));
        }

        [Test]
        public void Resolve_WhenTargetsContainNull_Throws()
        {
            var resolver = new FreezePulseResolver();

            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(
                CreateAbility(),
                new AlienState[] { null }));
        }

        [Test]
        public void Resolve_AppliesAbilityStatusEffectToLivingTargets()
        {
            var resolver = new FreezePulseResolver();
            var ability = CreateAbility();
            var target = CreateAlienState();

            var results = resolver.Resolve(ability, new[] { target });

            Assert.That(results, Has.Count.EqualTo(1));
            Assert.That(results[0].Source, Is.SameAs(target));
            Assert.That(results[0].Target.ActiveStatusEffects, Has.Count.EqualTo(1));
            Assert.That(results[0].Target.ActiveStatusEffects[0].Definition, Is.SameAs(ability.StatusEffect));
            Assert.That(results[0].Target.MovementSpeedMultiplier, Is.EqualTo(0.35f));
        }

        [Test]
        public void Resolve_WhenTargetIsDead_SkipsTarget()
        {
            var resolver = new FreezePulseResolver();
            var deadTarget = CreateAlienState().ApplyDamage(50);

            var results = resolver.Resolve(CreateAbility(), new[] { deadTarget });

            Assert.That(results, Is.Empty);
        }

        [Test]
        public void Resolve_WhenMultipleTargets_AppliesEffectToEachLivingTarget()
        {
            var resolver = new FreezePulseResolver();
            var first = CreateAlienState();
            var second = CreateAlienState();

            var results = resolver.Resolve(CreateAbility(), new[] { first, second });

            Assert.That(results, Has.Count.EqualTo(2));
            Assert.That(results[0].Source, Is.SameAs(first));
            Assert.That(results[1].Source, Is.SameAs(second));
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

        private static AlienState CreateAlienState()
        {
            return new AlienState(new AlienDefinition(
                "basic",
                50,
                1,
                10,
                new Dictionary<DamageType, float>()));
        }
    }
}
