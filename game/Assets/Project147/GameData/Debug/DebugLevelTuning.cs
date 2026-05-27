using System;
using Project147.GameCore.Level;
using UnityEngine;

namespace Project147.GameData.Debug
{
    [Serializable]
    public sealed class DebugLevelTuning
    {
        [SerializeField]
        private int startingCurrency = 150;

        [SerializeField]
        private int baseHealth = 10;

        [SerializeField]
        private int totalWaves = 5;

        [SerializeField]
        private float secondsBetweenSpawns = 0.8f;

        [SerializeField]
        private int startingWaveAlienCount = 4;

        [SerializeField]
        private int extraAliensPerWave = 2;

        [SerializeField]
        private int waveClearScrapReward = 25;

        [SerializeField]
        private int perfectWaveScrapBonus = 15;

        public int StartingCurrency
        {
            get { return startingCurrency; }
        }

        public int BaseHealth
        {
            get { return baseHealth; }
        }

        public int TotalWaves
        {
            get { return totalWaves; }
        }

        public int PerfectWaveScrapBonus
        {
            get { return perfectWaveScrapBonus; }
        }

        public WaveDefinition CreateWaveDefinition(
            int completedWaves,
            string basicAlienId,
            string fastAlienId,
            string armouredAlienId,
            string shieldedAlienId,
            string burrowerAlienId,
            string regeneratorAlienId,
            string bossAlienId)
        {
            return CreateWaveDefinition(
                completedWaves,
                totalWaves,
                basicAlienId,
                fastAlienId,
                armouredAlienId,
                shieldedAlienId,
                burrowerAlienId,
                regeneratorAlienId,
                bossAlienId);
        }

        public WaveDefinition CreateWaveDefinition(
            int completedWaves,
            int runTotalWaves,
            string basicAlienId,
            string fastAlienId,
            string armouredAlienId,
            string shieldedAlienId,
            string burrowerAlienId,
            string regeneratorAlienId,
            string bossAlienId)
        {
            return CreateWaveTuningDefinition().CreateWaveDefinition(
                completedWaves,
                runTotalWaves,
                new WaveAlienRoster(
                    basicAlienId,
                    fastAlienId,
                    armouredAlienId,
                    shieldedAlienId,
                    burrowerAlienId,
                    regeneratorAlienId,
                    bossAlienId));
        }

        public LevelWaveTuningDefinition CreateWaveTuningDefinition()
        {
            return new LevelWaveTuningDefinition(
                startingWaveAlienCount,
                extraAliensPerWave,
                secondsBetweenSpawns,
                waveClearScrapReward);
        }
    }
}
