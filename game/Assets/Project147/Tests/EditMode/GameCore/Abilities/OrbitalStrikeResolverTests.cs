using System;
using System.Collections.Generic;
using NUnit.Framework;
using Project147.GameCore.Abilities;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Abilities
{
    public sealed class OrbitalStrikeResolverTests
    {
        [Test]
        public void Constructor_WhenDamageResolverIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new OrbitalStrikeResolver(null));
        }

        [Test]
        public void Resolve_WhenAbilityIsNull_Throws()
        {
            var resolver = new OrbitalStrikeResolver(new DamageResolver());

            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(null, new[] { CreateAlienState() }));
        }

        [Test]
        public void Resolve_WhenTargetsAreNull_Throws()
        {
            var resolver = new OrbitalStrikeResolver(new DamageResolver());

            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(CreateAbility(), null));
        }

        [Test]
        public void Resolve_WhenTargetsContainNull_Throws()
        {
            var resolver = new OrbitalStrikeResolver(new DamageResolver());

            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(
                CreateAbility(),
                new AlienState[] { null }));
        }

        [Test]
        public void Resolve_WhenAbilityHasNoDamage_Throws()
        {
            var resolver = new OrbitalStrikeResolver(new DamageResolver());

            Assert.Throws<ArgumentException>(() => resolver.Resolve(
                new PlayerAbilityDefinition(
                    "freeze-pulse",
                    12,
                    new AlienStatusEffectDefinition(
                        "freeze-pulse-slow",
                        AlienStatusEffectType.Slow,
                        2,
                        0.35f)),
                new[] { CreateAlienState() }));
        }

        [Test]
        public void Resolve_DamagesEachLivingTarget()
        {
            var resolver = new OrbitalStrikeResolver(new DamageResolver());
            var first = CreateAlienState();
            var second = CreateAlienState();

            var results = resolver.Resolve(CreateAbility(), new[] { first, second });

            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results[0].Source, Is.SameAs(first));
            Assert.That(results[0].Target.CurrentHealth, Is.EqualTo(15));
            Assert.That(results[1].Source, Is.SameAs(second));
            Assert.That(results[1].Target.CurrentHealth, Is.EqualTo(15));
        }

        [Test]
        public void Resolve_AppliesTargetResistance()
        {
            var resolver = new OrbitalStrikeResolver(new DamageResolver());
            var resistantTarget = new AlienState(new AlienDefinition(
                "resistant",
                50,
                1,
                10,
                new Dictionary<DamageType, float>
                {
                    { DamageType.Energy, 0.5f }
                }));

            var results = resolver.Resolve(CreateAbility(), new[] { resistantTarget });

            Assert.That(results[0].Target.CurrentHealth, Is.EqualTo(32.5f));
        }

        [Test]
        public void Resolve_WhenTargetIsDead_SkipsTarget()
        {
            var resolver = new OrbitalStrikeResolver(new DamageResolver());
            var deadTarget = CreateAlienState().ApplyDamage(50);

            var results = resolver.Resolve(CreateAbility(), new[] { deadTarget });

            Assert.That(results, Is.Empty);
        }

        private static PlayerAbilityDefinition CreateAbility()
        {
            return new PlayerAbilityDefinition(
                "orbital-strike",
                18,
                35,
                DamageType.Energy);
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
