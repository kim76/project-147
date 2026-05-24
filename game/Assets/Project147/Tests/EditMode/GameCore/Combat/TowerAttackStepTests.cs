using System;
using System.Collections.Generic;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class TowerAttackStepTests
    {
        [Test]
        public void Constructor_WhenTargetSelectorIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new TowerAttackStep(null, new AttackResolver(new DamageResolver())));
        }

        [Test]
        public void Constructor_WhenAttackResolverIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new TowerAttackStep(new TowerTargetSelector(), null));
        }

        [Test]
        public void Step_WhenTowerIsNull_Throws()
        {
            var step = CreateStep();

            Assert.Throws<ArgumentNullException>(() => step.Step(null, new TargetCandidate[0]));
        }

        [Test]
        public void Step_WhenTowerIsOnCooldown_DoesNotFire()
        {
            var tower = new TowerState(CreateTower()).MarkFired();
            var step = CreateStep();

            var result = step.Step(tower, new[] { Candidate(100, 0, 1, 1) });

            Assert.That(result.DidFire, Is.False);
            Assert.That(result.Attack.HasValue, Is.False);
            Assert.That(result.Tower, Is.SameAs(tower));
        }

        [Test]
        public void Step_WhenNoTargetExists_DoesNotFireAndKeepsTowerReady()
        {
            var tower = new TowerState(CreateTower());
            var step = CreateStep();

            var result = step.Step(tower, new TargetCandidate[0]);

            Assert.That(result.DidFire, Is.False);
            Assert.That(result.Attack.HasValue, Is.False);
            Assert.That(result.Tower, Is.SameAs(tower));
            Assert.That(result.Tower.CanFire, Is.True);
        }

        [Test]
        public void Step_WhenTowerCanFireAndTargetExists_AttacksTargetAndStartsCooldown()
        {
            var tower = new TowerState(CreateTower());
            var step = CreateStep();

            var result = step.Step(tower, new[] { Candidate(100, 0, 1, 1) });

            Assert.That(result.DidFire, Is.True);
            Assert.That(result.Attack.HasValue, Is.True);
            Assert.That(result.Attack.Value.Target.CurrentHealth, Is.EqualTo(90));
            Assert.That(result.Tower.CanFire, Is.False);
            Assert.That(result.Tower.SecondsUntilReady, Is.EqualTo(0.5f));
        }

        private static TowerAttackStep CreateStep()
        {
            return new TowerAttackStep(new TowerTargetSelector(), new AttackResolver(new DamageResolver()));
        }

        private static TowerDefinition CreateTower()
        {
            return new TowerDefinition(
                "railgun-basic",
                100,
                3,
                2,
                10,
                DamageType.Kinetic,
                TowerTargetingMode.First);
        }

        private static TargetCandidate Candidate(
            float maxHealth,
            float damage,
            float pathProgress,
            float distanceToTower)
        {
            var alien = new AlienState(CreateAlien(maxHealth)).ApplyDamage(damage);
            return new TargetCandidate(alien, pathProgress, distanceToTower);
        }

        private static AlienDefinition CreateAlien(float maxHealth)
        {
            return new AlienDefinition(
                "runner-basic",
                maxHealth,
                1,
                5,
                new Dictionary<DamageType, float>());
        }
    }
}
