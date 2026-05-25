using System;
using System.Collections.Generic;
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
            if (completedWaves < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(completedWaves), "Completed waves cannot be negative.");
            }

            if (string.IsNullOrWhiteSpace(bossAlienId))
            {
                throw new ArgumentException("Boss alien id is required.", nameof(bossAlienId));
            }

            if (string.IsNullOrWhiteSpace(shieldedAlienId))
            {
                throw new ArgumentException("Shielded alien id is required.", nameof(shieldedAlienId));
            }

            if (string.IsNullOrWhiteSpace(burrowerAlienId))
            {
                throw new ArgumentException("Burrower alien id is required.", nameof(burrowerAlienId));
            }

            if (string.IsNullOrWhiteSpace(regeneratorAlienId))
            {
                throw new ArgumentException("Regenerator alien id is required.", nameof(regeneratorAlienId));
            }

            var totalAliens = startingWaveAlienCount + completedWaves * extraAliensPerWave;
            var hasBoss = completedWaves == totalWaves - 1;
            var fastAliens = completedWaves <= 0 ? 0 : Math.Max(1, totalAliens / 4);
            var armouredAliens = completedWaves < 2 ? 0 : Math.Max(1, totalAliens / 5);
            var shieldedAliens = completedWaves < 3 ? 0 : Math.Max(1, totalAliens / 6);
            var burrowerAliens = completedWaves < 2 ? 0 : Math.Max(1, totalAliens / 6);
            var regeneratorAliens = completedWaves < 3 ? 0 : Math.Max(1, totalAliens / 7);
            var bossAliens = hasBoss ? 1 : 0;
            var basicAliens = totalAliens
                - fastAliens
                - armouredAliens
                - shieldedAliens
                - burrowerAliens
                - regeneratorAliens
                - bossAliens;
            var groups = new List<WaveSpawnGroup>();

            AddGroup(groups, basicAlienId, basicAliens);
            AddGroup(groups, fastAlienId, fastAliens);
            AddGroup(groups, armouredAlienId, armouredAliens);
            AddGroup(groups, shieldedAlienId, shieldedAliens);
            AddGroup(groups, burrowerAlienId, burrowerAliens);
            AddGroup(groups, regeneratorAlienId, regeneratorAliens);
            AddGroup(groups, bossAlienId, bossAliens);

            return new WaveDefinition(
                new WaveComposition(groups),
                secondsBetweenSpawns,
                waveClearScrapReward);
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
