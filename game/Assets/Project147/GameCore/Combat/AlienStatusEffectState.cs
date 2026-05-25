using System;

namespace Project147.GameCore.Combat
{
    public sealed class AlienStatusEffectState
    {
        public AlienStatusEffectState(AlienStatusEffectDefinition definition)
            : this(definition, definition?.DurationSeconds ?? 0)
        {
        }

        private AlienStatusEffectState(AlienStatusEffectDefinition definition, float remainingSeconds)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));

            if (remainingSeconds < 0 || remainingSeconds > definition.DurationSeconds)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(remainingSeconds),
                    "Remaining status effect duration must be between zero and full duration.");
            }

            RemainingSeconds = remainingSeconds;
        }

        public AlienStatusEffectDefinition Definition { get; }

        public float RemainingSeconds { get; }

        public bool IsExpired
        {
            get { return RemainingSeconds <= 0; }
        }

        public AlienStatusEffectState Tick(float deltaSeconds)
        {
            if (deltaSeconds < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(deltaSeconds), "Delta seconds cannot be negative.");
            }

            return new AlienStatusEffectState(Definition, Math.Max(0, RemainingSeconds - deltaSeconds));
        }
    }
}
