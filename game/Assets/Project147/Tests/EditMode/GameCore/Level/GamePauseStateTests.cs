using System;
using NUnit.Framework;
using Project147.GameCore.Level;

namespace Project147.Tests.EditMode.GameCore.Level
{
    public sealed class GamePauseStateTests
    {
        [Test]
        public void Constructor_CreatesUnpausedState()
        {
            var state = new GamePauseState();

            Assert.That(state.IsPaused, Is.False);
            Assert.That(state.ScaleDeltaSeconds(0.5f), Is.EqualTo(0.5f));
        }

        [Test]
        public void Toggle_WhenUnpaused_ReturnsPausedState()
        {
            var state = new GamePauseState();

            var result = state.Toggle();

            Assert.That(result.IsPaused, Is.True);
            Assert.That(result.ScaleDeltaSeconds(0.5f), Is.EqualTo(0));
        }

        [Test]
        public void Toggle_WhenPaused_ReturnsUnpausedState()
        {
            var state = new GamePauseState().Toggle();

            var result = state.Toggle();

            Assert.That(result.IsPaused, Is.False);
            Assert.That(result.ScaleDeltaSeconds(0.5f), Is.EqualTo(0.5f));
        }

        [Test]
        public void Resume_WhenPaused_ReturnsUnpausedState()
        {
            var state = new GamePauseState().Toggle();

            var result = state.Resume();

            Assert.That(result.IsPaused, Is.False);
        }

        [Test]
        public void ScaleDeltaSeconds_WhenDeltaIsNegative_Throws()
        {
            var state = new GamePauseState();

            Assert.Throws<ArgumentOutOfRangeException>(() => state.ScaleDeltaSeconds(-0.01f));
        }
    }
}
