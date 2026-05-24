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
            var dead = Candidate(maxHealth: 100, damage: 100, pathProgress: 10, distanceToTower: 1);
            var alive = Candidate(maxHealth: 100, damage: 0, pathProgress: 5, distanceToTower: 2);
            var selector = new TowerTargetSelector();

            var result = selector.SelectTarget(new[] { dead, alive }, TowerTargetingMode.First);

            Assert.That(result, Is.EqualTo(alive));
        }

        [Test]
        public void SelectTarget_WhenModeIsFirst_SelectsHighestPathProgress()
        {
            var early = Candidate(pathProgress: 2, distanceToTower: 1, currentHealth: 50);
            var late = Candidate(pathProgress: 7, distanceToTower: 4, currentHealth: 20);
            var selector = new TowerTargetSelector();

            var result = selector.SelectTarget(new[] { early, late }, TowerTargetingMode.First);

            Assert.That(result, Is.EqualTo(late));
        }

        [Test]
        public void SelectTarget_WhenModeIsLast_SelectsLowestPathProgress()
        {
            var early = Candidate(pathProgress: 2, distanceToTower: 1, currentHealth: 50);
            var late = Candidate(pathProgress: 7, distanceToTower: 4, currentHealth: 20);
            var selector = new TowerTargetSelector();

            var result = selector.SelectTarget(new[] { late, early }, TowerTargetingMode.Last);

            Assert.That(result, Is.EqualTo(early));
        }

        [Test]
        public void SelectTarget_WhenModeIsClosest_SelectsLowestDistanceToTower()
        {
            var near = Candidate(pathProgress: 2, distanceToTower: 1, currentHealth: 50);
            var far = Candidate(pathProgress: 7, distanceToTower: 4, currentHealth: 20);
            var selector = new TowerTargetSelector();

            var result = selector.SelectTarget(new[] { far, near }, TowerTargetingMode.Closest);

            Assert.That(result, Is.EqualTo(near));
        }

        [Test]
        public void SelectTarget_WhenModeIsStrongest_SelectsHighestCurrentHealth()
        {
            var weak = Candidate(pathProgress: 7, distanceToTower: 1, currentHealth: 20);
            var strong = Candidate(pathProgress: 2, distanceToTower: 4, currentHealth: 50);
            var selector = new TowerTargetSelector();

            var result = selector.SelectTarget(new[] { weak, strong }, TowerTargetingMode.Strongest);

            Assert.That(result, Is.EqualTo(strong));
        }

        [Test]
        public void SelectTarget_WhenModeIsWeakest_SelectsLowestCurrentHealth()
        {
            var weak = Candidate(pathProgress: 7, distanceToTower: 1, currentHealth: 20);
            var strong = Candidate(pathProgress: 2, distanceToTower: 4, currentHealth: 50);
            var selector = new TowerTargetSelector();

            var result = selector.SelectTarget(new[] { strong, weak }, TowerTargetingMode.Weakest);

            Assert.That(result, Is.EqualTo(weak));
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
            return Candidate(100, 100 - currentHealth, pathProgress, distanceToTower);
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

