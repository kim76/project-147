using System;
using NUnit.Framework;
using Project147.GameCore.Level;

namespace Project147.Tests.EditMode.GameCore.Level
{
    public sealed class WaveSpawnStateTests
    {
        [Test]
        public void Constructor_WhenDefinitionIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new WaveSpawnState(null));
        }

        [Test]
        public void Constructor_WhenDefinitionIsValid_SetsInitialState()
        {
            var state = new WaveSpawnState(new WaveDefinition(3, 0.5f, 10));

            Assert.That(state.RemainingSpawns, Is.EqualTo(3));
            Assert.That(state.HasCompletedSpawning, Is.False);
        }

        [Test]
        public void Tick_WhenWaveStarts_SpawnsFirstAlienImmediately()
        {
            var state = new WaveSpawnState(new WaveDefinition(3, 0.5f, 10));

            var result = state.Tick(0);

            Assert.That(result.SpawnCount, Is.EqualTo(1));
            Assert.That(result.State.RemainingSpawns, Is.EqualTo(2));
            Assert.That(result.State.HasCompletedSpawning, Is.False);
        }

        [Test]
        public void Tick_WhenIntervalHasNotElapsed_SpawnsNothing()
        {
            var state = new WaveSpawnState(new WaveDefinition(3, 0.5f, 10))
                .Tick(0)
                .State;

            var result = state.Tick(0.25f);

            Assert.That(result.SpawnCount, Is.EqualTo(0));
            Assert.That(result.State.RemainingSpawns, Is.EqualTo(2));
        }

        [Test]
        public void Tick_WhenIntervalElapses_SpawnsNextAlien()
        {
            var state = new WaveSpawnState(new WaveDefinition(3, 0.5f, 10))
                .Tick(0)
                .State;

            var result = state.Tick(0.5f);

            Assert.That(result.SpawnCount, Is.EqualTo(1));
            Assert.That(result.State.RemainingSpawns, Is.EqualTo(1));
        }

        [Test]
        public void Tick_WhenDeltaCoversMultipleIntervals_SpawnsMultipleAliens()
        {
            var state = new WaveSpawnState(new WaveDefinition(4, 0.5f, 10))
                .Tick(0)
                .State;

            var result = state.Tick(1.6f);

            Assert.That(result.SpawnCount, Is.EqualTo(3));
            Assert.That(result.State.RemainingSpawns, Is.EqualTo(0));
            Assert.That(result.State.HasCompletedSpawning, Is.True);
        }

        [Test]
        public void Tick_WhenSpawningIsComplete_SpawnsNothing()
        {
            var state = new WaveSpawnState(new WaveDefinition(1, 0.5f, 10))
                .Tick(0)
                .State;

            var result = state.Tick(10);

            Assert.That(result.SpawnCount, Is.EqualTo(0));
            Assert.That(result.State.RemainingSpawns, Is.EqualTo(0));
            Assert.That(result.State.HasCompletedSpawning, Is.True);
        }

        [Test]
        public void Tick_WhenDeltaIsNegative_Throws()
        {
            var state = new WaveSpawnState(new WaveDefinition(1, 0.5f, 10));

            Assert.Throws<ArgumentOutOfRangeException>(() => state.Tick(-0.01f));
        }
    }
}
