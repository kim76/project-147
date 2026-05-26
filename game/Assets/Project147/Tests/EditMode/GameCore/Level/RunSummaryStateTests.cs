using System;
using NUnit.Framework;
using Project147.GameCore.Level;

namespace Project147.Tests.EditMode.GameCore.Level
{
    public sealed class RunSummaryStateTests
    {
        [Test]
        public void Constructor_CreatesEmptyInProgressSummary()
        {
            var summary = new RunSummaryState();

            Assert.That(summary.Outcome, Is.EqualTo(RunOutcome.InProgress));
            Assert.That(summary.AliensDestroyed, Is.EqualTo(0));
            Assert.That(summary.AliensLeaked, Is.EqualTo(0));
            Assert.That(summary.WavesCleared, Is.EqualTo(0));
            Assert.That(summary.PerfectWaves, Is.EqualTo(0));
            Assert.That(summary.ScrapEarned, Is.EqualTo(0));
            Assert.That(summary.ShieldBurstUses, Is.EqualTo(0));
            Assert.That(summary.TowerOverchargeUses, Is.EqualTo(0));
            Assert.That(summary.StarRating, Is.EqualTo(0));
        }

        [Test]
        public void RecordAlienDestroyed_IncrementsDestroyedAndScrap()
        {
            var summary = new RunSummaryState();

            var result = summary.RecordAlienDestroyed(15);

            Assert.That(result.AliensDestroyed, Is.EqualTo(1));
            Assert.That(result.ScrapEarned, Is.EqualTo(15));
            Assert.That(summary.AliensDestroyed, Is.EqualTo(0));
        }

        [Test]
        public void RecordAlienLeaked_IncrementsLeaked()
        {
            var summary = new RunSummaryState();

            var result = summary.RecordAlienLeaked();

            Assert.That(result.AliensLeaked, Is.EqualTo(1));
        }

        [Test]
        public void RecordWaveCleared_IncrementsWavesPerfectWavesAndScrap()
        {
            var summary = new RunSummaryState();

            var result = summary.RecordWaveCleared(40, true);

            Assert.That(result.WavesCleared, Is.EqualTo(1));
            Assert.That(result.PerfectWaves, Is.EqualTo(1));
            Assert.That(result.ScrapEarned, Is.EqualTo(40));
        }

        [Test]
        public void RecordRewardChosen_IncrementsRewardChoices()
        {
            var summary = new RunSummaryState();

            var result = summary.RecordRewardChosen();

            Assert.That(result.RewardsChosen, Is.EqualTo(1));
        }

        [Test]
        public void RecordFreezePulseUsed_IncrementsFreezePulseUses()
        {
            var summary = new RunSummaryState();

            var result = summary.RecordFreezePulseUsed();

            Assert.That(result.FreezePulseUses, Is.EqualTo(1));
        }

        [Test]
        public void RecordOrbitalStrikeUsed_IncrementsOrbitalStrikeUses()
        {
            var summary = new RunSummaryState();

            var result = summary.RecordOrbitalStrikeUsed();

            Assert.That(result.OrbitalStrikeUses, Is.EqualTo(1));
        }

        [Test]
        public void RecordShieldBurstUsed_IncrementsShieldBurstUses()
        {
            var summary = new RunSummaryState();

            var result = summary.RecordShieldBurstUsed();

            Assert.That(result.ShieldBurstUses, Is.EqualTo(1));
        }

        [Test]
        public void RecordTowerOverchargeUsed_IncrementsTowerOverchargeUses()
        {
            var summary = new RunSummaryState();

            var result = summary.RecordTowerOverchargeUsed();

            Assert.That(result.TowerOverchargeUses, Is.EqualTo(1));
        }

        [Test]
        public void Complete_WhenOutcomeIsVictory_StoresOutcome()
        {
            var summary = new RunSummaryState();

            var result = summary.Complete(RunOutcome.Victory);

            Assert.That(result.Outcome, Is.EqualTo(RunOutcome.Victory));
        }

        [Test]
        public void StarRating_WhenVictoryHasNoLeaks_ReturnsThreeStars()
        {
            var summary = new RunSummaryState()
                .RecordWaveCleared(25, true)
                .Complete(RunOutcome.Victory);

            Assert.That(summary.StarRating, Is.EqualTo(3));
        }

        [Test]
        public void StarRating_WhenVictoryHasFewLeaks_ReturnsTwoStars()
        {
            var summary = new RunSummaryState()
                .RecordAlienLeaked()
                .RecordAlienLeaked()
                .RecordWaveCleared(25, false)
                .Complete(RunOutcome.Victory);

            Assert.That(summary.StarRating, Is.EqualTo(2));
        }

        [Test]
        public void StarRating_WhenVictoryHasManyLeaks_ReturnsOneStar()
        {
            var summary = new RunSummaryState()
                .RecordAlienLeaked()
                .RecordAlienLeaked()
                .RecordAlienLeaked()
                .RecordWaveCleared(25, false)
                .Complete(RunOutcome.Victory);

            Assert.That(summary.StarRating, Is.EqualTo(1));
        }

        [Test]
        public void StarRating_WhenDefeat_ReturnsZeroStars()
        {
            var summary = new RunSummaryState()
                .RecordWaveCleared(25, true)
                .Complete(RunOutcome.Defeat);

            Assert.That(summary.StarRating, Is.EqualTo(0));
        }

        [Test]
        public void Complete_WhenOutcomeIsInProgress_Throws()
        {
            var summary = new RunSummaryState();

            Assert.Throws<ArgumentException>(() => summary.Complete(RunOutcome.InProgress));
        }

        [Test]
        public void RecordAlienDestroyed_WhenScrapIsNegative_Throws()
        {
            var summary = new RunSummaryState();

            Assert.Throws<ArgumentOutOfRangeException>(() => summary.RecordAlienDestroyed(-1));
        }
    }
}
