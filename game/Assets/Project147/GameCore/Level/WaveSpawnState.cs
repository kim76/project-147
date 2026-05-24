using System;

namespace Project147.GameCore.Level
{
    public sealed class WaveSpawnState : IEquatable<WaveSpawnState>
    {
        public WaveSpawnState(WaveDefinition definition)
            : this(definition, definition?.AlienCount ?? 0, 0)
        {
        }

        private WaveSpawnState(WaveDefinition definition, int remainingSpawns, float secondsUntilNextSpawn)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            if (remainingSpawns < 0 || remainingSpawns > definition.AlienCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(remainingSpawns),
                    "Remaining spawns must be between zero and wave alien count.");
            }

            if (secondsUntilNextSpawn < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(secondsUntilNextSpawn),
                    "Seconds until next spawn cannot be negative.");
            }

            Definition = definition;
            RemainingSpawns = remainingSpawns;
            SecondsUntilNextSpawn = secondsUntilNextSpawn;
        }

        public WaveDefinition Definition { get; }

        public int RemainingSpawns { get; }

        public float SecondsUntilNextSpawn { get; }

        public bool HasCompletedSpawning
        {
            get { return RemainingSpawns <= 0; }
        }

        public WaveSpawnResult Tick(float deltaSeconds)
        {
            if (deltaSeconds < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(deltaSeconds), "Delta seconds cannot be negative.");
            }

            if (HasCompletedSpawning)
            {
                return new WaveSpawnResult(this, 0);
            }

            var secondsUntilNextSpawn = SecondsUntilNextSpawn - deltaSeconds;
            var remainingSpawns = RemainingSpawns;
            var spawnCount = 0;

            while (remainingSpawns > 0 && secondsUntilNextSpawn <= 0)
            {
                spawnCount++;
                remainingSpawns--;
                secondsUntilNextSpawn += Definition.SecondsBetweenSpawns;
            }

            if (remainingSpawns <= 0)
            {
                secondsUntilNextSpawn = 0;
            }

            var state = new WaveSpawnState(Definition, remainingSpawns, secondsUntilNextSpawn);
            return new WaveSpawnResult(state, spawnCount);
        }

        public bool Equals(WaveSpawnState other)
        {
            return other != null
                && Equals(Definition, other.Definition)
                && RemainingSpawns == other.RemainingSpawns
                && SecondsUntilNextSpawn.Equals(other.SecondsUntilNextSpawn);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as WaveSpawnState);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Definition, RemainingSpawns, SecondsUntilNextSpawn);
        }
    }
}
