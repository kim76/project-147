using System;
using System.Collections.Generic;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class TowerTargetSelectorTests
    {
        [Test]
        public void SelectTarget_WhenCandidatesAreNull_Throws()
        {
            var selector = new TowerTargetSelector();

            Assert.Throws<ArgumentNullException>(() => selector.SelectTarget(null, TowerTargetingMode.First));
        }

        [Test]
        public void SelectTarget_WhenNoCandidates_ReturnsNull()
        {
            var selector = new TowerTargetSelector();

            var result = selector.SelectTarget(new TargetCandidate[0], TowerTargetingMode.First);

            Assert.That(result.HasValue, Is.False);
        }

        [Test]
        public void SelectTarget_IgnoresDeadCandidates()
        {
            var dead = CreateCandidate(maxHealth: 100, damage: 100, pathProgress: 10, distanceToTower: 1);
            var alive = CreateCandidate(maxHealth: 100, damage: 0, pathProgress: 5, distanceToTower: 2);
            var selector = new TowerTargetSelector();

            var result = selector.SelectTarget(new[] { dead, alive }, TowerTargetingMode.First);

            Assert.That(result.HasValue, Is.True);
            AssertTargetCandidate(result.Value, alive);
        }

        [Test]
        public void SelectTarget_IgnoresUntargetableCandidates()
        {
            var untargetable = Candidate(pathProgress: 1, distanceToTower: 1, currentHealth: 100, targetableAfterPathProgress: 3);
            var targetable = Candidate(pathProgress: 0.5f, distanceToTower: 2, currentHealth: 80);
            var selector = new TowerTargetSelector();

            var result = selector.SelectTarget(new[] { untargetable, targetable }, TowerTargetingMode.First);

            Assert.That(untargetable.IsTargetable, Is.False);
            Assert.That(targetable.IsTargetable, Is.True);
            Assert.That(result.HasValue, Is.True);
            AssertTargetCandidate(result.Value, targetable);
        }

        [Test]
        public void SelectTarget_WhenModeIsFirst_SelectsHighestPathProgress()
        {
            var early = Candidate(pathProgress: 2, distanceToTower: 1, currentHealth: 50);
            var late = Candidate(pathProgress: 7, distanceToTower: 4, currentHealth: 20);
            var selector = new TowerTargetSelector();

            var result = selector.SelectTarget(new[] { early, late }, TowerTargetingMode.First);

            Assert.That(early.IsTargetable, Is.True);
            Assert.That(late.IsTargetable, Is.True);
            Assert.That(result.HasValue, Is.True);
            AssertTargetCandidate(result.Value, late);
        }

        [Test]
        public void SelectTarget_WhenModeIsLast_SelectsLowestPathProgress()
        {
            var early = Candidate(pathProgress: 2, distanceToTower: 1, currentHealth: 50);
            var late = Candidate(pathProgress: 7, distanceToTower: 4, currentHealth: 20);
            var selector = new TowerTargetSelector();

            var result = selector.SelectTarget(new[] { late, early }, TowerTargetingMode.Last);

            Assert.That(early.IsTargetable, Is.True);
            Assert.That(late.IsTargetable, Is.True);
            Assert.That(result.HasValue, Is.True);
            AssertTargetCandidate(result.Value, early);
        }

        [Test]
        public void SelectTarget_WhenModeIsClosest_SelectsLowestDistanceToTower()
        {
            var near = Candidate(pathProgress: 2, distanceToTower: 1, currentHealth: 50);
            var far = Candidate(pathProgress: 7, distanceToTower: 4, currentHealth: 20);
            var selector = new TowerTargetSelector();

            var result = selector.SelectTarget(new[] { far, near }, TowerTargetingMode.Closest);

            Assert.That(result.HasValue, Is.True);
            AssertTargetCandidate(result.Value, near);
        }

        [Test]
        public void SelectTarget_WhenModeIsStrongest_SelectsHighestCurrentHealth()
        {
            var weak = Candidate(pathProgress: 7, distanceToTower: 1, currentHealth: 20);
            var strong = Candidate(pathProgress: 2, distanceToTower: 4, currentHealth: 50);
            var selector = new TowerTargetSelector();

            var result = selector.SelectTarget(new[] { weak, strong }, TowerTargetingMode.Strongest);

            Assert.That(weak.IsTargetable, Is.True);
            Assert.That(strong.IsTargetable, Is.True);
            Assert.That(result.HasValue, Is.True);
            AssertTargetCandidate(result.Value, strong);
        }

        [Test]
        public void SelectTarget_WhenModeIsWeakest_SelectsLowestCurrentHealth()
        {
            var weak = Candidate(pathProgress: 7, distanceToTower: 1, currentHealth: 20);
            var strong = Candidate(pathProgress: 2, distanceToTower: 4, currentHealth: 50);
            var selector = new TowerTargetSelector();

            var result = selector.SelectTarget(new[] { strong, weak }, TowerTargetingMode.Weakest);

            Assert.That(weak.IsTargetable, Is.True);
            Assert.That(strong.IsTargetable, Is.True);
            Assert.That(result.HasValue, Is.True);
            AssertTargetCandidate(result.Value, weak);
        }

        [Test]
        public void SelectTarget_WhenModeIsUnknown_Throws()
        {
            var selector = new TowerTargetSelector();

            Assert.Throws<ArgumentOutOfRangeException>(() => selector.SelectTarget(
                new[] { Candidate(pathProgress: 1, distanceToTower: 1, currentHealth: 10) },
                (TowerTargetingMode)999));
        }

        private static TargetCandidate Candidate(float pathProgress, float distanceToTower, float currentHealth)
        {
            return CreateCandidate(100, 100 - currentHealth, pathProgress, distanceToTower);
        }

        private static void AssertTargetCandidate(TargetCandidate actual, TargetCandidate expected)
        {
            Assert.That(actual.PathProgress, Is.EqualTo(expected.PathProgress));
            Assert.That(actual.DistanceToTower, Is.EqualTo(expected.DistanceToTower));
            Assert.That(actual.Alien.CurrentHealth, Is.EqualTo(expected.Alien.CurrentHealth));
            Assert.That(actual.Alien.Definition.Id, Is.EqualTo(expected.Alien.Definition.Id));
        }

        private static TargetCandidate Candidate(
            float pathProgress,
            float distanceToTower,
            float currentHealth,
            float targetableAfterPathProgress)
        {
            return CreateCandidate(
                100,
                100 - currentHealth,
                pathProgress,
                distanceToTower,
                targetableAfterPathProgress);
        }

        private static TargetCandidate CreateCandidate(
            float maxHealth,
            float damage,
            float pathProgress,
            float distanceToTower,
            float targetableAfterPathProgress = 0)
        {
            var alien = new AlienState(CreateAlien(maxHealth, targetableAfterPathProgress)).ApplyDamage(damage);
            return new TargetCandidate(alien, pathProgress, distanceToTower);
        }

        private static AlienDefinition CreateAlien(float maxHealth, float targetableAfterPathProgress = 0)
        {
            return new AlienDefinition(
                "runner-basic",
                maxHealth,
                1,
                5,
                new Dictionary<DamageType, float>(),
                0,
                0,
                targetableAfterPathProgress);
        }
    }
}
