using System;
using System.Collections.Generic;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class AttackResolverTests
    {
        [Test]
        public void Constructor_WhenDamageResolverIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new AttackResolver(null));
        }

        [Test]
        public void Resolve_WhenTowerIsNull_Throws()
        {
            var resolver = new AttackResolver(new DamageResolver());

            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(null, new AlienState(CreateAlien(100))));
        }

        [Test]
        public void Resolve_WhenTargetIsNull_Throws()
        {
            var resolver = new AttackResolver(new DamageResolver());

            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(CreateTower(10), null));
        }

        [Test]
        public void Resolve_AppliesTowerDamageToAlienHealth()
        {
            var resolver = new AttackResolver(new DamageResolver());
            var tower = CreateTower(30);
            var target = new AlienState(CreateAlien(100));

            var result = resolver.Resolve(tower, target);

            Assert.That(result.Damage.FinalAmount, Is.EqualTo(30));
            Assert.That(result.Target.CurrentHealth, Is.EqualTo(70));
            Assert.That(result.KilledTarget, Is.False);
            Assert.That(target.CurrentHealth, Is.EqualTo(100));
        }

        [Test]
        public void Resolve_AppliesResistanceBeforeHealthDamage()
        {
            var resolver = new AttackResolver(new DamageResolver());
            var tower = CreateTower(40);
            var target = new AlienState(CreateAlien(
                100,
                new Dictionary<DamageType, float>
                {
                    { DamageType.Kinetic, 0.25f }
                }));

            var result = resolver.Resolve(tower, target);

            Assert.That(result.Damage.FinalAmount, Is.EqualTo(30));
            Assert.That(result.Target.CurrentHealth, Is.EqualTo(70));
        }

        [Test]
        public void Resolve_WhenDamageKillsTarget_ReturnsKilledTarget()
        {
            var resolver = new AttackResolver(new DamageResolver());
            var tower = CreateTower(100);
            var target = new AlienState(CreateAlien(50));

            var result = resolver.Resolve(tower, target);

            Assert.That(result.Target.IsAlive, Is.False);
            Assert.That(result.KilledTarget, Is.True);
        }

        [Test]
        public void Resolve_WhenTargetIsAlreadyDead_DoesNotApplyMoreDamage()
        {
            var resolver = new AttackResolver(new DamageResolver());
            var tower = CreateTower(100);
            var target = new AlienState(CreateAlien(50)).ApplyDamage(50);

            var result = resolver.Resolve(tower, target);

            Assert.That(result.Damage.FinalAmount, Is.EqualTo(0));
            Assert.That(result.Target.CurrentHealth, Is.EqualTo(0));
            Assert.That(result.KilledTarget, Is.False);
        }

        private static TowerDefinition CreateTower(float damage)
        {
            return new TowerDefinition(
                "railgun-basic",
                100,
                3,
                1,
                damage,
                DamageType.Kinetic,
                TowerTargetingMode.First);
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

