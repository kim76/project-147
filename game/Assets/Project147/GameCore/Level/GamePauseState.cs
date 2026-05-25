namespace Project147.GameCore.Level
{
    public sealed class GamePauseState
    {
        public GamePauseState()
            : this(false)
        {
        }

        private GamePauseState(bool isPaused)
        {
            IsPaused = isPaused;
        }

        public bool IsPaused { get; }

        public GamePauseState Toggle()
        {
            return new GamePauseState(!IsPaused);
        }

        public GamePauseState Resume()
        {
            return IsPaused ? new GamePauseState(false) : this;
        }

        public float ScaleDeltaSeconds(float deltaSeconds)
        {
            if (deltaSeconds < 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(deltaSeconds), "Delta seconds cannot be negative.");
            }

            return IsPaused ? 0 : deltaSeconds;
        }
    }
}
