using Project147.GameCore.Combat;
using Project147.GameCore.Level;
using UnityEngine;

namespace Project147.GameData.Debug
{
    [CreateAssetMenu(menuName = "Project147/Debug/First Slice Config", fileName = "DebugFirstSliceConfig")]
    public sealed class DebugFirstSliceConfig : ScriptableObject
    {
        [Header("Level")]
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

        [Header("Tower")]
        [SerializeField]
        private string towerId = "debug-railgun";

        [SerializeField]
        private int towerCost = 50;

        [SerializeField]
        private float towerRange = 2.35f;

        [SerializeField]
        private float towerFireRatePerSecond = 1.25f;

        [SerializeField]
        private float towerDamage = 24;

        [SerializeField]
        private float towerCriticalChance;

        [SerializeField]
        private float towerCriticalDamageMultiplier = 1.5f;

        [SerializeField]
        private DamageType towerDamageType = DamageType.Kinetic;

        [SerializeField]
        private TowerTargetingMode towerTargetingMode = TowerTargetingMode.First;

        [Header("Tower Upgrade")]
        [SerializeField]
        private int maxTowerLevel = 3;

        [SerializeField]
        private string towerUpgradeId = "debug-railgun-upgrade";

        [SerializeField]
        private int towerUpgradeCost = 75;

        [SerializeField]
        private float towerUpgradeDamageMultiplier = 1.35f;

        [SerializeField]
        private float towerUpgradeFireRateMultiplier = 1.15f;

        [SerializeField]
        private float towerUpgradeRangeBonus = 0.2f;

        [SerializeField]
        private float towerUpgradeCriticalChanceBonus = 0.05f;

        [SerializeField]
        private float towerUpgradeCriticalDamageMultiplierBonus = 0.15f;

        [Header("Alien")]
        [SerializeField]
        private string alienId = "debug-runner";

        [SerializeField]
        private float alienHealth = 60;

        [SerializeField]
        private float alienSpeedCellsPerSecond = 1.6f;

        [SerializeField]
        private int alienReward = 15;

        [SerializeField]
        private float alienDodgeChance;

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

        public float SecondsBetweenSpawns
        {
            get { return secondsBetweenSpawns; }
        }

        public int StartingWaveAlienCount
        {
            get { return startingWaveAlienCount; }
        }

        public int ExtraAliensPerWave
        {
            get { return extraAliensPerWave; }
        }

        public int WaveClearScrapReward
        {
            get { return waveClearScrapReward; }
        }

        public int MaxTowerLevel
        {
            get { return maxTowerLevel; }
        }

        public WaveDefinition CreateWaveDefinition(int completedWaves)
        {
            if (completedWaves < 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(completedWaves), "Completed waves cannot be negative.");
            }

            return new WaveDefinition(
                startingWaveAlienCount + completedWaves * extraAliensPerWave,
                secondsBetweenSpawns,
                waveClearScrapReward);
        }

        public TowerDefinition CreateTowerDefinition()
        {
            return new TowerDefinition(
                towerId,
                towerCost,
                towerRange,
                towerFireRatePerSecond,
                towerDamage,
                towerDamageType,
                towerTargetingMode,
                towerCriticalChance,
                towerCriticalDamageMultiplier);
        }

        public TowerUpgradeDefinition CreateTowerUpgradeDefinition()
        {
            return new TowerUpgradeDefinition(
                towerUpgradeId,
                towerUpgradeCost,
                towerUpgradeDamageMultiplier,
                towerUpgradeFireRateMultiplier,
                towerUpgradeRangeBonus,
                towerUpgradeCriticalChanceBonus,
                towerUpgradeCriticalDamageMultiplierBonus);
        }

        public AlienDefinition CreateAlienDefinition()
        {
            return new AlienDefinition(
                alienId,
                alienHealth,
                alienSpeedCellsPerSecond,
                alienReward,
                new System.Collections.Generic.Dictionary<DamageType, float>(),
                alienDodgeChance);
        }
    }
}
