using System;
using NUnit.Framework;
using Project147.GameCore.Level;

namespace Project147.Tests.EditMode.GameCore.Level
{
    public sealed class AlienRunSummaryStateTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_StoresValues()
        {
            var summary = new AlienRunSummaryState(
                AlienRunOutcome.AlienVictory,
                "swarm",
                "speed",
                8,
                5,
                3,
                10,
                0,
                3);

            Assert.That(summary.Outcome, Is.EqualTo(AlienRunOutcome.AlienVictory));
            Assert.That(summary.SquadPlanId, Is.EqualTo("swarm"));
            Assert.That(summary.UpgradePlanId, Is.EqualTo("speed"));
            Assert.That(summary.SquadSize, Is.EqualTo(8));
            Assert.That(summary.AliensLeaked, Is.EqualTo(5));
            Assert.That(summary.AliensDestroyed, Is.EqualTo(3));
            Assert.That(summary.BaseDamageDealt, Is.EqualTo(10));
            Assert.That(summary.AliensStopped, Is.EqualTo(3));
            Assert.That(summary.DefenceTowerCount, Is.EqualTo(3));
        }

        [Test]
        public void Score_WhenAlienVictory_IncludesVictoryBonus()
        {
            var summary = new AlienRunSummaryState(
                AlienRunOutcome.AlienVictory,
                "swarm",
                "speed",
                8,
                5,
                3,
                10,
                0,
                3);

            Assert.That(summary.Score, Is.EqualTo(1750));
            Assert.That(summary.StarRating, Is.EqualTo(3));
        }

        [Test]
        public void StarRating_WhenDefenceHeld_ReflectsBaseDamage()
        {
            var noDamage = CreateDefenceHeldSummary(10);
            var someDamage = CreateDefenceHeldSummary(9);
            var heavyDamage = CreateDefenceHeldSummary(5);

            Assert.That(noDamage.StarRating, Is.EqualTo(0));
            Assert.That(someDamage.StarRating, Is.EqualTo(1));
            Assert.That(heavyDamage.StarRating, Is.EqualTo(2));
        }

        [Test]
        public void Constructor_WhenPlanIdsAreMissing_Throws()
        {
            Assert.Throws<ArgumentException>(() => new AlienRunSummaryState(
                AlienRunOutcome.DefenceHeld,
                "",
                "speed",
                8,
                0,
                8,
                10,
                10,
                3));
            Assert.Throws<ArgumentException>(() => new AlienRunSummaryState(
                AlienRunOutcome.DefenceHeld,
                "swarm",
                "",
                8,
                0,
                8,
                10,
                10,
                3));
        }

        [Test]
        public void Constructor_WhenCountsAreInvalid_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AlienRunSummaryState(
                AlienRunOutcome.DefenceHeld,
                "swarm",
                "speed",
                0,
                0,
                0,
                10,
                10,
                3));
            Assert.Throws<ArgumentOutOfRangeException>(() => new AlienRunSummaryState(
                AlienRunOutcome.DefenceHeld,
                "swarm",
                "speed",
                8,
                9,
                0,
                10,
                10,
                3));
            Assert.Throws<ArgumentOutOfRangeException>(() => new AlienRunSummaryState(
                AlienRunOutcome.DefenceHeld,
                "swarm",
                "speed",
                8,
                0,
                9,
                10,
                10,
                3));
        }

        private static AlienRunSummaryState CreateDefenceHeldSummary(int remainingBaseHealth)
        {
            var baseDamage = 10 - remainingBaseHealth;

            return new AlienRunSummaryState(
                AlienRunOutcome.DefenceHeld,
                "swarm",
                "speed",
                8,
                baseDamage,
                8 - baseDamage,
                10,
                remainingBaseHealth,
                3);
        }
    }
}
