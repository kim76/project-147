using System;

namespace Project147.GameCore.Level
{
    public sealed class GameSpeedState
    {
        private static readonly float[] Multipliers = { 1f, 2f, 3f };

        public GameSpeedState()
            : this(0)
        {
        }

        private GameSpeedState(int selectedIndex)
        {
            if (selectedIndex < 0 || selectedIndex >= Multipliers.Length)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(selectedIndex),
                    "Selected speed index is outside the configured speed list.");
            }

            SelectedIndex = selectedIndex;
        }

        public int SelectedIndex { get; }

        public float Multiplier
        {
            get { return Multipliers[SelectedIndex]; }
        }

        public GameSpeedState SelectNext()
        {
            return new GameSpeedState((SelectedIndex + 1) % Multipliers.Length);
        }

        public float ScaleDeltaSeconds(float deltaSeconds)
        {
            if (deltaSeconds < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(deltaSeconds), "Delta seconds cannot be negative.");
            }

            return deltaSeconds * Multiplier;
        }
    }
}
