using System;

namespace Project147.PlatformServices.Audio
{
    public sealed class AudioEventDefinition
    {
        public AudioEventDefinition(string id, float defaultVolume, float defaultPitch)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Audio event id is required.", nameof(id));
            }

            if (defaultVolume < 0 || defaultVolume > 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(defaultVolume),
                    "Audio event default volume must be between zero and one.");
            }

            if (defaultPitch <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(defaultPitch),
                    "Audio event default pitch must be greater than zero.");
            }

            Id = id;
            DefaultVolume = defaultVolume;
            DefaultPitch = defaultPitch;
        }

        public string Id { get; }

        public float DefaultVolume { get; }

        public float DefaultPitch { get; }
    }
}
