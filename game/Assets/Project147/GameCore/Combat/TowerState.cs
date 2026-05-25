using System;

namespace Project147.GameCore.Combat
{
    public sealed class TowerState
    {
        private static readonly TowerTargetingMode[] TargetingModeOrder =
        {
            TowerTargetingMode.First,
            TowerTargetingMode.Last,
            TowerTargetingMode.Closest,
            TowerTargetingMode.Strongest,
            TowerTargetingMode.Weakest
        };

        public TowerState(TowerDefinition definition)
            : this(definition, 1, 0, definition?.DefaultTargetingMode ?? TowerTargetingMode.First)
        {
        }

        private TowerState(
            TowerDefinition definition,
            int level,
            float secondsUntilReady,
            TowerTargetingMode targetingMode)
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

            if (!Enum.IsDefined(typeof(TowerTargetingMode), targetingMode))
            {
                throw new ArgumentOutOfRangeException(nameof(targetingMode), targetingMode, "Unknown targeting mode.");
            }

            Level = level;
            SecondsUntilReady = secondsUntilReady;
            TargetingMode = targetingMode;
        }

        public TowerDefinition Definition { get; }

        public int Level { get; }

        public float SecondsUntilReady { get; }

        public TowerTargetingMode TargetingMode { get; }

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

            return new TowerState(Definition, Level, Math.Max(0, SecondsUntilReady - deltaSeconds), TargetingMode);
        }

        public TowerState MarkFired()
        {
            return new TowerState(Definition, Level, SecondsPerShot(), TargetingMode);
        }

        public TowerState Upgrade(TowerUpgradeDefinition upgrade)
        {
            if (upgrade == null)
            {
                throw new ArgumentNullException(nameof(upgrade));
            }

            return new TowerState(upgrade.ApplyTo(Definition), Level + 1, SecondsUntilReady, TargetingMode);
        }

        public TowerState SelectNextTargetingMode()
        {
            var currentIndex = Array.IndexOf(TargetingModeOrder, TargetingMode);
            var nextIndex = (currentIndex + 1) % TargetingModeOrder.Length;
            return new TowerState(Definition, Level, SecondsUntilReady, TargetingModeOrder[nextIndex]);
        }

        private float SecondsPerShot()
        {
            return 1 / Definition.FireRatePerSecond;
        }
    }
}
