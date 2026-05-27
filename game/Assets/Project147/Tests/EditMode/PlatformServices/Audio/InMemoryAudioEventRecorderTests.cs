using System;
using NUnit.Framework;
using Project147.PlatformServices.Audio;

namespace Project147.Tests.EditMode.PlatformServices.Audio
{
    public sealed class InMemoryAudioEventRecorderTests
    {
        [Test]
        public void Play_WhenEventIsKnown_RecordsDefaultAudioEvent()
        {
            var recorder = CreateRecorder();

            recorder.Play(Project147AudioEvents.TowerPlaced);

            Assert.That(recorder.Records, Has.Count.EqualTo(1));
            Assert.That(recorder.Records[0].EventId, Is.EqualTo(Project147AudioEvents.TowerPlaced));
            Assert.That(recorder.Records[0].Volume, Is.EqualTo(0.7f));
        }

        [Test]
        public void Constructor_WhenDefinitionsAreNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new InMemoryAudioEventRecorder(null));
        }

        [Test]
        public void Constructor_WhenDefinitionsContainNull_Throws()
        {
            Assert.Throws<ArgumentException>(() => new InMemoryAudioEventRecorder(
                new AudioEventDefinition[] { null }));
        }

        [Test]
        public void Constructor_WhenDefinitionIdsRepeat_Throws()
        {
            Assert.Throws<ArgumentException>(() => new InMemoryAudioEventRecorder(
                new[]
                {
                    new AudioEventDefinition(Project147AudioEvents.TowerPlaced, 0.7f, 1),
                    new AudioEventDefinition(Project147AudioEvents.TowerPlaced, 0.7f, 1)
                }));
        }

        [Test]
        public void Play_WhenEventIsUnknown_Throws()
        {
            var recorder = CreateRecorder();

            Assert.Throws<ArgumentException>(() => recorder.Play("unknown"));
        }

        private static InMemoryAudioEventRecorder CreateRecorder()
        {
            return new InMemoryAudioEventRecorder(new Project147AudioCatalog().CreateGameplayEvents());
        }
    }
}
