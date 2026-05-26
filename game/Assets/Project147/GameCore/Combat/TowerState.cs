using System;
using System.Collections.Generic;

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
            : this(
                definition,
                1,
                0,
                definition?.DefaultTargetingMode ?? TowerTargetingMode.First,
                0,
                new List<string>())
        {
        }

        private TowerState(
            TowerDefinition definition,
            int level,
            float secondsUntilReady,
            TowerTargetingMode targetingMode,
            int upgradeSpend,
            IReadOnlyList<string> upgradeHistory)
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

            if (upgradeSpend < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(upgradeSpend), "Upgrade spend cannot be negative.");
            }

            if (upgradeHistory == null)
            {
                throw new ArgumentNullException(nameof(upgradeHistory));
            }

            foreach (var upgradeId in upgradeHistory)
            {
                if (string.IsNullOrWhiteSpace(upgradeId))
                {
                    throw new ArgumentException("Upgrade history cannot contain empty upgrade ids.", nameof(upgradeHistory));
                }
            }

            if (!Enum.IsDefined(typeof(TowerTargetingMode), targetingMode))
            {
                throw new ArgumentOutOfRangeException(nameof(targetingMode), targetingMode, "Unknown targeting mode.");
            }

            Level = level;
            SecondsUntilReady = secondsUntilReady;
            TargetingMode = targetingMode;
            UpgradeSpend = upgradeSpend;
            UpgradeHistory = new List<string>(upgradeHistory);
        }

        public TowerDefinition Definition { get; }

        public int Level { get; }

        public float SecondsUntilReady { get; }

        public TowerTargetingMode TargetingMode { get; }

        public int UpgradeSpend { get; }

        public IReadOnlyList<string> UpgradeHistory { get; }

        public int TotalSpend
        {
            get { return Definition.Cost + UpgradeSpend; }
        }

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

            return new TowerState(
                Definition,
                Level,
                Math.Max(0, SecondsUntilReady - deltaSeconds),
                TargetingMode,
                UpgradeSpend,
                UpgradeHistory);
        }

        public TowerState MarkFired()
        {
            return MarkFired(Definition.FireRatePerSecond);
        }

        public TowerState MarkFired(float fireRatePerSecond)
        {
            if (fireRatePerSecond <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(fireRatePerSecond),
                    "Tower fire rate must be greater than zero.");
            }

            return new TowerState(Definition, Level, SecondsPerShot(fireRatePerSecond), TargetingMode, UpgradeSpend, UpgradeHistory);
        }

        public TowerState Upgrade(TowerUpgradeDefinition upgrade)
        {
            if (upgrade == null)
            {
                throw new ArgumentNullException(nameof(upgrade));
            }

            var upgradeHistory = new List<string>(UpgradeHistory)
            {
                upgrade.Id
            };

            return new TowerState(
                upgrade.ApplyTo(Definition),
                Level + 1,
                SecondsUntilReady,
                TargetingMode,
                UpgradeSpend + upgrade.Cost,
                upgradeHistory);
        }

        public TowerState SelectNextTargetingMode()
        {
            var currentIndex = Array.IndexOf(TargetingModeOrder, TargetingMode);
            var nextIndex = (currentIndex + 1) % TargetingModeOrder.Length;
            return new TowerState(
                Definition,
                Level,
                SecondsUntilReady,
                TargetingModeOrder[nextIndex],
                UpgradeSpend,
                UpgradeHistory);
        }

        private static float SecondsPerShot(float fireRatePerSecond)
        {
            return 1 / fireRatePerSecond;
        }
    }
}
