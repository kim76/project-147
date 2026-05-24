using System;

namespace Project147.GameCore.Combat
{
    public sealed class TowerState
    {
        public TowerState(TowerDefinition definition)
            : this(definition, 0)
        {
        }

        private TowerState(TowerDefinition definition, float secondsUntilReady)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));

            if (secondsUntilReady < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(secondsUntilReady),
                    "Seconds until ready cannot be negative.");
            }

            SecondsUntilReady = secondsUntilReady;
        }

        public TowerDefinition Definition { get; }

        public float SecondsUntilReady { get; }

        public bool CanFire
        {
            get { return SecondsUntilReady <= 0; }
        }

        public TowerState Tick(float deltaSeconds)
        {
            if (deltaSeconds < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(deltaSeconds), "Delta seconds cannot be negative.");
            }

            return new TowerState(Definition, Math.Max(0, SecondsUntilReady - deltaSeconds));
        }

        public TowerState MarkFired()
        {
            return new TowerState(Definition, SecondsPerShot());
        }

        private float SecondsPerShot()
        {
            return 1 / Definition.FireRatePerSecond;
        }
    }
}

