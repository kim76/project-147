using System;
using NUnit.Framework;
using Project147.GameCore.Level;

namespace Project147.Tests.EditMode.GameCore.Level
{
    public sealed class GameSpeedStateTests
    {
        [Test]
        public void Constructor_CreatesNormalSpeed()
        {
            var speed = new GameSpeedState();

            Assert.That(speed.SelectedIndex, Is.EqualTo(0));
            Assert.That(speed.Multiplier, Is.EqualTo(1));
        }

        [Test]
        public void SelectNext_CyclesThroughConfiguredSpeeds()
        {
            var speed = new GameSpeedState();

            var doubleSpeed = speed.SelectNext();
            var tripleSpeed = doubleSpeed.SelectNext();
            var normalSpeed = tripleSpeed.SelectNext();

            Assert.That(doubleSpeed.Multiplier, Is.EqualTo(2));
            Assert.That(tripleSpeed.Multiplier, Is.EqualTo(3));
            Assert.That(normalSpeed.Multiplier, Is.EqualTo(1));
        }

        [Test]
        public void ScaleDeltaSeconds_MultipliesByCurrentSpeed()
        {
            var speed = new GameSpeedState()
                .SelectNext()
                .SelectNext();

            var result = speed.ScaleDeltaSeconds(0.25f);

            Assert.That(result, Is.EqualTo(0.75f));
        }

        [Test]
        public void ScaleDeltaSeconds_WhenDeltaIsNegative_Throws()
        {
            var speed = new GameSpeedState();

            Assert.Throws<ArgumentOutOfRangeException>(() => speed.ScaleDeltaSeconds(-0.01f));
        }
    }
}
