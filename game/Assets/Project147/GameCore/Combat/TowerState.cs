using System;

namespace Project147.GameCore.Combat
{
    public sealed class TowerState
    {
        public TowerState(TowerDefinition definition)
            : this(definition, 1, 0)
        {
        }

        private TowerState(TowerDefinition definition, int level, float secondsUntilReady)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));

            if (level <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(level), "Tower level must be greater than zero.");
            }

            if (secondsUntilReady < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(secondsUntilReady),
                    "Seconds until ready cannot be negative.");
            }

            Level = level;
            SecondsUntilReady = secondsUntilReady;
        }

        public TowerDefinition Definition { get; }

        public int Level { get; }

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

            return new TowerState(Definition, Level, Math.Max(0, SecondsUntilReady - deltaSeconds));
        }

        public TowerState MarkFired()
        {
            return new TowerState(Definition, Level, SecondsPerShot());
        }

        public TowerState Upgrade(TowerUpgradeDefinition upgrade)
        {
            if (upgrade == null)
            {
                throw new ArgumentNullException(nameof(upgrade));
            }

            return new TowerState(upgrade.ApplyTo(Definition), Level + 1, SecondsUntilReady);
        }

        private float SecondsPerShot()
        {
            return 1 / Definition.FireRatePerSecond;
        }
    }
}
