using System;
using System.Collections.Generic;

namespace Project147.GameCore.Level
{
    public sealed class LevelWaveTuningDefinition
    {
        public LevelWaveTuningDefinition(
            int startingWaveAlienCount,
            int extraAliensPerWave,
            float secondsBetweenSpawns,
            int waveClearScrapReward)
            : this(
                startingWaveAlienCount,
                extraAliensPerWave,
                secondsBetweenSpawns,
                waveClearScrapReward,
                1,
                2,
                3,
                2,
                3)
        {
        }

        public LevelWaveTuningDefinition(
            int startingWaveAlienCount,
            int extraAliensPerWave,
            float secondsBetweenSpawns,
            int waveClearScrapReward,
            int fastStartWave,
            int armouredStartWave,
            int shieldedStartWave,
            int burrowerStartWave,
            int regeneratorStartWave)
        {
            if (startingWaveAlienCount <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(startingWaveAlienCount),
                    "Starting wave alien count must be greater than zero.");
            }

            if (extraAliensPerWave < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(extraAliensPerWave),
                    "Extra aliens per wave cannot be negative.");
            }

            if (secondsBetweenSpawns <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(secondsBetweenSpawns),
                    "Seconds between spawns must be greater than zero.");
            }

            if (waveClearScrapReward < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(waveClearScrapReward),
                    "Wave clear scrap reward cannot be negative.");
            }

            ValidateStartWave(fastStartWave, nameof(fastStartWave));
            ValidateStartWave(armouredStartWave, nameof(armouredStartWave));
            ValidateStartWave(shieldedStartWave, nameof(shieldedStartWave));
            ValidateStartWave(burrowerStartWave, nameof(burrowerStartWave));
            ValidateStartWave(regeneratorStartWave, nameof(regeneratorStartWave));

            StartingWaveAlienCount = startingWaveAlienCount;
            ExtraAliensPerWave = extraAliensPerWave;
            SecondsBetweenSpawns = secondsBetweenSpawns;
            WaveClearScrapReward = waveClearScrapReward;
            FastStartWave = fastStartWave;
            ArmouredStartWave = armouredStartWave;
            ShieldedStartWave = shieldedStartWave;
            BurrowerStartWave = burrowerStartWave;
            RegeneratorStartWave = regeneratorStartWave;
        }

        public int StartingWaveAlienCount { get; }

        public int ExtraAliensPerWave { get; }

        public float SecondsBetweenSpawns { get; }

        public int WaveClearScrapReward { get; }

        public int FastStartWave { get; }

        public int ArmouredStartWave { get; }

        public int ShieldedStartWave { get; }

        public int BurrowerStartWave { get; }

        public int RegeneratorStartWave { get; }

        public WaveDefinition CreateWaveDefinition(
            int completedWaves,
            int runTotalWaves,
            WaveAlienRoster roster)
        {
            if (completedWaves < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(completedWaves), "Completed waves cannot be negative.");
            }

            if (runTotalWaves <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(runTotalWaves), "Run total waves must be greater than zero.");
            }

            if (roster == null)
            {
                throw new ArgumentNullException(nameof(roster));
            }

            var totalAliens = StartingWaveAlienCount + completedWaves * ExtraAliensPerWave;
            var hasBoss = completedWaves == runTotalWaves - 1;
            var fastAliens = completedWaves < FastStartWave ? 0 : Math.Max(1, totalAliens / 4);
            var armouredAliens = completedWaves < ArmouredStartWave ? 0 : Math.Max(1, totalAliens / 5);
            var shieldedAliens = completedWaves < ShieldedStartWave ? 0 : Math.Max(1, totalAliens / 6);
            var burrowerAliens = completedWaves < BurrowerStartWave ? 0 : Math.Max(1, totalAliens / 6);
            var regeneratorAliens = completedWaves < RegeneratorStartWave ? 0 : Math.Max(1, totalAliens / 7);
            var bossAliens = hasBoss ? 1 : 0;
            var basicAliens = totalAliens
                - fastAliens
                - armouredAliens
                - shieldedAliens
                - burrowerAliens
                - regeneratorAliens
                - bossAliens;
            var groups = new List<WaveSpawnGroup>();

            AddGroup(groups, roster.BasicAlienId, basicAliens);
            AddGroup(groups, roster.FastAlienId, fastAliens);
            AddGroup(groups, roster.ArmouredAlienId, armouredAliens);
            AddGroup(groups, roster.ShieldedAlienId, shieldedAliens);
            AddGroup(groups, roster.BurrowerAlienId, burrowerAliens);
            AddGroup(groups, roster.RegeneratorAlienId, regeneratorAliens);
            AddGroup(groups, roster.BossAlienId, bossAliens);

            return new WaveDefinition(
                new WaveComposition(groups),
                SecondsBetweenSpawns,
                WaveClearScrapReward);
        }

        private static void ValidateStartWave(int wave, string parameterName)
        {
            if (wave < 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, "Special alien start wave cannot be negative.");
            }
        }

        private static void AddGroup(ICollection<WaveSpawnGroup> groups, string alienId, int count)
        {
            if (count > 0)
            {
                groups.Add(new WaveSpawnGroup(alienId, count));
            }
        }
    }
}
