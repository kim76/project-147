using System;
using System.Collections.Generic;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class SplashDamageResolverTests
    {
        [Test]
        public void Constructor_WhenDamageResolverIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new SplashDamageResolver(null));
        }

        [Test]
        public void Resolve_WhenTowerIsNull_Throws()
        {
            var resolver = new SplashDamageResolver(new DamageResolver());

            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(
                null,
                new[] { new SplashDamageCandidate(new AlienState(CreateAlien(100)), 1) }));
        }

        [Test]
        public void Resolve_WhenCandidatesAreNull_Throws()
        {
            var resolver = new SplashDamageResolver(new DamageResolver());

            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(CreateSplashTower(), null));
        }

        [Test]
        public void Resolve_WhenTowerHasNoSplash_ReturnsNoResults()
        {
            var resolver = new SplashDamageResolver(new DamageResolver());
            var tower = new TowerDefinition(
                "railgun-basic",
                50,
                3,
                1,
                40,
                DamageType.Kinetic,
                TowerTargetingMode.First);

            var results = resolver.Resolve(
                tower,
                new[] { new SplashDamageCandidate(new AlienState(CreateAlien(100)), 0.5f) });

            Assert.That(results, Is.Empty);
        }

        [Test]
        public void Resolve_AppliesSplashDamageToTargetsInsideRadius()
        {
            var resolver = new SplashDamageResolver(new DamageResolver());
            var target = new AlienState(CreateAlien(100));

            var results = resolver.Resolve(
                CreateSplashTower(),
                new[] { new SplashDamageCandidate(target, 1) });

            Assert.That(results, Has.Count.EqualTo(1));
            Assert.That(results[0].Source, Is.SameAs(target));
            Assert.That(results[0].Damage.BaseAmount, Is.EqualTo(20));
            Assert.That(results[0].Damage.FinalAmount, Is.EqualTo(20));
            Assert.That(results[0].Target.CurrentHealth, Is.EqualTo(80));
        }

        [Test]
        public void Resolve_AppliesResistanceBeforeSplashDamage()
        {
            var resolver = new SplashDamageResolver(new DamageResolver());
            var target = new AlienState(CreateAlien(
                100,
                new Dictionary<DamageType, float>
                {
                    { DamageType.Explosive, 0.25f }
                }));

            var results = resolver.Resolve(
                CreateSplashTower(),
                new[] { new SplashDamageCandidate(target, 1) });

            Assert.That(results[0].Damage.FinalAmount, Is.EqualTo(15));
            Assert.That(results[0].Target.CurrentHealth, Is.EqualTo(85));
        }

        [Test]
        public void Resolve_WhenCandidateIsOutsideRadius_SkipsCandidate()
        {
            var resolver = new SplashDamageResolver(new DamageResolver());
            var target = new AlienState(CreateAlien(100));

            var results = resolver.Resolve(
                CreateSplashTower(),
                new[] { new SplashDamageCandidate(target, 1.51f) });

            Assert.That(results, Is.Empty);
        }

        [Test]
        public void Resolve_WhenCandidateIsDead_SkipsCandidate()
        {
            var resolver = new SplashDamageResolver(new DamageResolver());
            var target = new AlienState(CreateAlien(100)).ApplyDamage(100);

            var results = resolver.Resolve(
                CreateSplashTower(),
                new[] { new SplashDamageCandidate(target, 1) });

            Assert.That(results, Is.Empty);
        }

        [Test]
        public void Constructor_WhenTargetIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new SplashDamageCandidate(null, 1));
        }

        [Test]
        public void Constructor_WhenDistanceIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new SplashDamageCandidate(
                new AlienState(CreateAlien(100)),
                -0.01f));
        }

        private static TowerDefinition CreateSplashTower()
        {
            return new TowerDefinition(
                "mortar-basic",
                70,
                3,
                0.8f,
                40,
                DamageType.Explosive,
                TowerTargetingMode.Strongest,
                0,
                1,
                1.5f,
                0.5f);
        }

        private static AlienDefinition CreateAlien(float maxHealth)
        {
            return CreateAlien(maxHealth, new Dictionary<DamageType, float>());
        }

        private static AlienDefinition CreateAlien(
            float maxHealth,
            IReadOnlyDictionary<DamageType, float> resistances)
        {
            return new AlienDefinition(
                "runner-basic",
                maxHealth,
                1,
                5,
                resistances);
        }
    }
}
