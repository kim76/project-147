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
        public void Constructor_WhenChanceRollerIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new AttackResolver(new DamageResolver(), null));
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

        [Test]
        public void Resolve_WhenTargetDodges_AppliesNoDamage()
        {
            var resolver = new AttackResolver(new DamageResolver(), new FixedCombatChanceRoller(true));
            var tower = CreateTower(100);
            var target = new AlienState(CreateAlien(50, new Dictionary<DamageType, float>(), 1));

            var result = resolver.Resolve(tower, target);

            Assert.That(result.Damage.WasDodged, Is.True);
            Assert.That(result.Damage.WasCritical, Is.False);
            Assert.That(result.Damage.FinalAmount, Is.EqualTo(0));
            Assert.That(result.Target.CurrentHealth, Is.EqualTo(50));
            Assert.That(result.KilledTarget, Is.False);
        }

        [Test]
        public void Resolve_WhenAttackCrits_MultipliesBaseDamageBeforeResistance()
        {
            var resolver = new AttackResolver(new DamageResolver(), new FixedCombatChanceRoller(false, true));
            var tower = CreateTower(40, 1, 2);
            var target = new AlienState(CreateAlien(
                100,
                new Dictionary<DamageType, float>
                {
                    { DamageType.Kinetic, 0.25f }
                },
                0));

            var result = resolver.Resolve(tower, target);

            Assert.That(result.Damage.WasDodged, Is.False);
            Assert.That(result.Damage.WasCritical, Is.True);
            Assert.That(result.Damage.BaseAmount, Is.EqualTo(80));
            Assert.That(result.Damage.FinalAmount, Is.EqualTo(60));
            Assert.That(result.Target.CurrentHealth, Is.EqualTo(40));
        }

        private static TowerDefinition CreateTower(float damage)
        {
            return CreateTower(damage, 0, 1);
        }

        private static TowerDefinition CreateTower(
            float damage,
            float criticalChance,
            float criticalDamageMultiplier)
        {
            return new TowerDefinition(
                "railgun-basic",
                100,
                3,
                1,
                damage,
                DamageType.Kinetic,
                TowerTargetingMode.First,
                criticalChance,
                criticalDamageMultiplier);
        }

        private static AlienDefinition CreateAlien(float maxHealth)
        {
            return CreateAlien(maxHealth, new Dictionary<DamageType, float>(), 0);
        }

        private static AlienDefinition CreateAlien(
            float maxHealth,
            IReadOnlyDictionary<DamageType, float> resistances,
            float dodgeChance = 0)
        {
            return new AlienDefinition(
                "runner-basic",
                maxHealth,
                1,
                5,
                resistances,
                dodgeChance);
        }

        private sealed class FixedCombatChanceRoller : ICombatChanceRoller
        {
            private readonly Queue<bool> outcomes;

            public FixedCombatChanceRoller(params bool[] outcomes)
            {
                this.outcomes = new Queue<bool>(outcomes);
            }

            public bool Succeeds(float chance)
            {
                return outcomes.Count > 0 && outcomes.Dequeue();
            }
        }
    }
}
