using System;

namespace Project147.GameCore.Level
{
    public sealed class WaveSpawnState : IEquatable<WaveSpawnState>
    {
        public WaveSpawnState(WaveDefinition definition)
            : this(definition, 0, 0)
        {
        }

        private WaveSpawnState(WaveDefinition definition, int nextSpawnIndex, float secondsUntilNextSpawn)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            if (nextSpawnIndex < 0 || nextSpawnIndex > definition.AlienCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(nextSpawnIndex),
                    "Next spawn index must be between zero and wave alien count.");
            }

            if (secondsUntilNextSpawn < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(secondsUntilNextSpawn),
                    "Seconds until next spawn cannot be negative.");
            }

            Definition = definition;
            NextSpawnIndex = nextSpawnIndex;
            SecondsUntilNextSpawn = secondsUntilNextSpawn;
        }

        public WaveDefinition Definition { get; }

        public int NextSpawnIndex { get; }

        public int RemainingSpawns
        {
            get { return Definition.AlienCount - NextSpawnIndex; }
        }

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
            var nextSpawnIndex = NextSpawnIndex;
            var spawnEntries = new System.Collections.Generic.List<WaveSpawnEntry>();

            while (nextSpawnIndex < Definition.AlienCount && secondsUntilNextSpawn <= 0)
            {
                spawnEntries.Add(Definition.SpawnEntries[nextSpawnIndex]);
                nextSpawnIndex++;
                secondsUntilNextSpawn += Definition.SecondsBetweenSpawns;
            }

            if (nextSpawnIndex >= Definition.AlienCount)
            {
                secondsUntilNextSpawn = 0;
            }

            var state = new WaveSpawnState(Definition, nextSpawnIndex, secondsUntilNextSpawn);
            return new WaveSpawnResult(state, spawnEntries);
        }

        public bool Equals(WaveSpawnState other)
        {
            return other != null
                && Equals(Definition, other.Definition)
                && NextSpawnIndex == other.NextSpawnIndex
                && SecondsUntilNextSpawn.Equals(other.SecondsUntilNextSpawn);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as WaveSpawnState);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Definition, NextSpawnIndex, SecondsUntilNextSpawn);
        }
    }
}
