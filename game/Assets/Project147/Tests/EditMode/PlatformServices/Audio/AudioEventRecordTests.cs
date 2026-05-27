using System;
using NUnit.Framework;
using Project147.PlatformServices.Audio;

namespace Project147.Tests.EditMode.PlatformServices.Audio
{
    public sealed class AudioEventRecordTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_StoresValues()
        {
            var record = new AudioEventRecord(Project147AudioEvents.TowerPlaced, 0.7f, 1.1f);

            Assert.That(record.EventId, Is.EqualTo(Project147AudioEvents.TowerPlaced));
            Assert.That(record.Volume, Is.EqualTo(0.7f));
            Assert.That(record.Pitch, Is.EqualTo(1.1f));
        }

        [Test]
        public void Constructor_WhenIdIsMissing_Throws()
        {
            Assert.Throws<ArgumentException>(() => new AudioEventRecord("", 0.7f, 1));
        }

        [Test]
        public void Constructor_WhenVolumeIsOutsideRange_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AudioEventRecord(Project147AudioEvents.TowerPlaced, 1.01f, 1));
        }

        [Test]
        public void Constructor_WhenPitchIsZero_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AudioEventRecord(Project147AudioEvents.TowerPlaced, 0.7f, 0));
        }
    }
}
