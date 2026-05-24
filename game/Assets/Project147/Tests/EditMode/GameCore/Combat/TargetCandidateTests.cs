using System;
using System.Collections.Generic;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class TargetCandidateTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_StoresValues()
        {
            var alien = new AlienState(CreateAlien(100));

            var candidate = new TargetCandidate(alien, 3.5f, 2.25f);

            Assert.That(candidate.Alien, Is.SameAs(alien));
            Assert.That(candidate.PathProgress, Is.EqualTo(3.5f));
            Assert.That(candidate.DistanceToTower, Is.EqualTo(2.25f));
        }

        [Test]
        public void Constructor_WhenAlienIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new TargetCandidate(null, 0, 0));
        }

        [Test]
        public void Constructor_WhenPathProgressIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TargetCandidate(
                new AlienState(CreateAlien(100)),
                -1,
                0));
        }

        [Test]
        public void Constructor_WhenDistanceToTowerIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TargetCandidate(
                new AlienState(CreateAlien(100)),
                0,
                -1));
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

