using System;
using NUnit.Framework;
using Project147.GameCore.Level;

namespace Project147.Tests.EditMode.GameCore.Level
{
    public sealed class LevelProgressStateTests
    {
        [Test]
        public void Constructor_CreatesEmptyProgress()
        {
            var progress = new LevelProgressState();

            Assert.That(progress.RunsCompleted, Is.EqualTo(0));
            Assert.That(progress.Victories, Is.EqualTo(0));
            Assert.That(progress.BestStars, Is.EqualTo(0));
            Assert.That(progress.BestWavesCleared, Is.EqualTo(0));
            Assert.That(progress.BestPerfectWaves, Is.EqualTo(0));
            Assert.That(progress.HasThreeStarClear, Is.False);
        }

        [Test]
        public void ApplyRunSummary_WhenVictory_RecordsStarsAndVictory()
        {
            var progress = new LevelProgressState();
            var summary = CreateVictory(stars: 3);

            var result = progress.ApplyRunSummary(summary);

            Assert.That(result.State.RunsCompleted, Is.EqualTo(1));
            Assert.That(result.State.Victories, Is.EqualTo(1));
            Assert.That(result.State.BestStars, Is.EqualTo(3));
            Assert.That(result.State.BestWavesCleared, Is.EqualTo(summary.WavesCleared));
            Assert.That(result.State.BestPerfectWaves, Is.EqualTo(summary.PerfectWaves));
            Assert.That(result.State.HasThreeStarClear, Is.True);
            Assert.That(result.ImprovedStars, Is.True);
            Assert.That(result.HasAnyImprovement, Is.True);
        }

        [Test]
        public void ApplyRunSummary_WhenDefeat_RecordsRunAndBestWaves()
        {
            var progress = new LevelProgressState();
            var summary = new RunSummaryState()
                .RecordWaveCleared(25, true)
                .RecordWaveCleared(25, false)
                .Complete(RunOutcome.Defeat);

            var result = progress.ApplyRunSummary(summary);

            Assert.That(result.State.RunsCompleted, Is.EqualTo(1));
            Assert.That(result.State.Victories, Is.EqualTo(0));
            Assert.That(result.State.BestStars, Is.EqualTo(0));
            Assert.That(result.State.BestWavesCleared, Is.EqualTo(2));
            Assert.That(result.ImprovedStars, Is.False);
            Assert.That(result.ImprovedWavesCleared, Is.True);
        }

        [Test]
        public void ApplyRunSummary_WhenLowerStarVictory_DoesNotReduceBestStars()
        {
            var progress = new LevelProgressState()
                .ApplyRunSummary(CreateVictory(stars: 3))
                .State;

            var result = progress.ApplyRunSummary(CreateVictory(stars: 1));

            Assert.That(result.State.RunsCompleted, Is.EqualTo(2));
            Assert.That(result.State.Victories, Is.EqualTo(2));
            Assert.That(result.State.BestStars, Is.EqualTo(3));
            Assert.That(result.ImprovedStars, Is.False);
        }

        [Test]
        public void ApplyRunSummary_WhenSummaryIsNull_Throws()
        {
            var progress = new LevelProgressState();

            Assert.Throws<ArgumentNullException>(() => progress.ApplyRunSummary(null));
        }

        [Test]
        public void ApplyRunSummary_WhenSummaryIsInProgress_Throws()
        {
            var progress = new LevelProgressState();

            Assert.Throws<ArgumentException>(() => progress.ApplyRunSummary(new RunSummaryState()));
        }

        private static RunSummaryState CreateVictory(int stars)
        {
            var summary = new RunSummaryState()
                .RecordWaveCleared(25, true)
                .RecordWaveCleared(25, true)
                .RecordWaveCleared(25, true);

            for (var leakIndex = 0; leakIndex < LeaksForStars(stars); leakIndex++)
            {
                summary = summary.RecordAlienLeaked();
            }

            return summary.Complete(RunOutcome.Victory);
        }

        private static int LeaksForStars(int stars)
        {
            switch (stars)
            {
                case 3:
                    return 0;
                case 2:
                    return 2;
                case 1:
                    return 3;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stars), "Stars must be between one and three.");
            }
        }
    }
}
