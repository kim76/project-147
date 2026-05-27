using System;
using NUnit.Framework;
using Project147.PlatformServices.Audio;

namespace Project147.Tests.EditMode.PlatformServices.Audio
{
    public sealed class AudioEventDefinitionTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_StoresValues()
        {
            var definition = new AudioEventDefinition(Project147AudioEvents.TowerPlaced, 0.7f, 1.1f);

            Assert.That(definition.Id, Is.EqualTo(Project147AudioEvents.TowerPlaced));
            Assert.That(definition.DefaultVolume, Is.EqualTo(0.7f));
            Assert.That(definition.DefaultPitch, Is.EqualTo(1.1f));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WhenIdIsMissing_Throws(string id)
        {
            Assert.Throws<ArgumentException>(() => new AudioEventDefinition(id, 0.7f, 1));
        }

        [TestCase(-0.01f)]
        [TestCase(1.01f)]
        public void Constructor_WhenVolumeIsOutsideRange_Throws(float volume)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AudioEventDefinition(Project147AudioEvents.TowerPlaced, volume, 1));
        }

        [TestCase(0)]
        [TestCase(-0.01f)]
        public void Constructor_WhenPitchIsZeroOrLess_Throws(float pitch)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AudioEventDefinition(Project147AudioEvents.TowerPlaced, 0.7f, pitch));
        }
    }
}
