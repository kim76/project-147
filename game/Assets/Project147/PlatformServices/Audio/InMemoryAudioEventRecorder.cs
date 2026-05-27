using System;
using System.Collections.Generic;
using System.Linq;

namespace Project147.PlatformServices.Audio
{
    public sealed class InMemoryAudioEventRecorder
    {
        private readonly IReadOnlyList<AudioEventDefinition> definitions;
        private readonly List<AudioEventRecord> records = new List<AudioEventRecord>();

        public InMemoryAudioEventRecorder(IReadOnlyList<AudioEventDefinition> definitions)
        {
            if (definitions == null)
            {
                throw new ArgumentNullException(nameof(definitions));
            }

            if (definitions.Any(definition => definition == null))
            {
                throw new ArgumentException("Audio event definitions cannot contain null entries.", nameof(definitions));
            }

            if (definitions.Select(definition => definition.Id).Distinct(StringComparer.Ordinal).Count() != definitions.Count)
            {
                throw new ArgumentException("Audio event definitions must have unique ids.", nameof(definitions));
            }

            this.definitions = new List<AudioEventDefinition>(definitions);
        }

        public IReadOnlyList<AudioEventRecord> Records
        {
            get { return records; }
        }

        public void Play(string eventId)
        {
            var definition = definitions.SingleOrDefault(candidate => candidate.Id == eventId);

            if (definition == null)
            {
                throw new ArgumentException($"Unknown audio event '{eventId}'.", nameof(eventId));
            }

            records.Add(new AudioEventRecord(
                definition.Id,
                definition.DefaultVolume,
                definition.DefaultPitch));
        }
    }
}
