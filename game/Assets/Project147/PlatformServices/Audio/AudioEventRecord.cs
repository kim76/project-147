using System;

namespace Project147.PlatformServices.Audio
{
    public sealed class AudioEventRecord
    {
        public AudioEventRecord(string eventId, float volume, float pitch)
        {
            if (string.IsNullOrWhiteSpace(eventId))
            {
                throw new ArgumentException("Audio event record id is required.", nameof(eventId));
            }

            if (volume < 0 || volume > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(volume), "Audio event volume must be between zero and one.");
            }

            if (pitch <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pitch), "Audio event pitch must be greater than zero.");
            }

            EventId = eventId;
            Volume = volume;
            Pitch = pitch;
        }

        public string EventId { get; }

        public float Volume { get; }

        public float Pitch { get; }
    }
}
