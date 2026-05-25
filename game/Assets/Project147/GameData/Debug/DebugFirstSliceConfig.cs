using System.Collections.Generic;
using Project147.GameCore.Abilities;
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

        [SerializeField]
        private int perfectWaveScrapBonus = 15;

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

        [Header("Second Tower")]
        [SerializeField]
        private string secondTowerId = "debug-mortar";

        [SerializeField]
        private int secondTowerCost = 70;

        [SerializeField]
        private float secondTowerRange = 2.8f;

        [SerializeField]
        private float secondTowerFireRatePerSecond = 0.75f;

        [SerializeField]
        private float secondTowerDamage = 38;

        [SerializeField]
        private float secondTowerCriticalChance = 0.1f;

        [SerializeField]
        private float secondTowerCriticalDamageMultiplier = 1.8f;

        [SerializeField]
        private float secondTowerSplashRadius = 1.25f;

        [SerializeField]
        private float secondTowerSplashDamageMultiplier = 0.45f;

        [SerializeField]
        private DamageType secondTowerDamageType = DamageType.Explosive;

        [SerializeField]
        private TowerTargetingMode secondTowerTargetingMode = TowerTargetingMode.Strongest;

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

        [Header("Tower Status Effect")]
        [SerializeField]
        private string towerStatusEffectId = "debug-frost-slow";

        [SerializeField]
        private float towerStatusEffectDurationSeconds = 1.4f;

        [SerializeField]
        private float towerStatusEffectMovementSpeedMultiplier = 0.7f;

        [Header("Player Ability")]
        [SerializeField]
        private string freezePulseAbilityId = "debug-freeze-pulse";

        [SerializeField]
        private float freezePulseCooldownSeconds = 12;

        [SerializeField]
        private string freezePulseStatusEffectId = "debug-freeze-pulse-slow";

        [SerializeField]
        private float freezePulseDurationSeconds = 2.2f;

        [SerializeField]
        private float freezePulseMovementSpeedMultiplier = 0.35f;

        [Header("Basic Alien")]
        [SerializeField]
        private string basicAlienId = "debug-basic";

        [SerializeField]
        private float basicAlienHealth = 60;

        [SerializeField]
        private float basicAlienSpeedCellsPerSecond = 1.45f;

        [SerializeField]
        private int basicAlienReward = 15;

        [SerializeField]
        private float basicAlienDodgeChance;

        [Header("Fast Alien")]
        [SerializeField]
        private string fastAlienId = "debug-fast";

        [SerializeField]
        private float fastAlienHealth = 38;

        [SerializeField]
        private float fastAlienSpeedCellsPerSecond = 2.25f;

        [SerializeField]
        private int fastAlienReward = 12;

        [SerializeField]
        private float fastAlienDodgeChance = 0.12f;

        [Header("Armoured Alien")]
        [SerializeField]
        private string armouredAlienId = "debug-armoured";

        [SerializeField]
        private float armouredAlienHealth = 115;

        [SerializeField]
        private float armouredAlienSpeedCellsPerSecond = 0.95f;

        [SerializeField]
        private int armouredAlienReward = 26;

        [SerializeField]
        private float armouredAlienDodgeChance;

        [SerializeField]
        private DamageType armouredAlienResistanceDamageType = DamageType.Kinetic;

        [SerializeField]
        private float armouredAlienResistance = 0.35f;

        [Header("Alien Upgrade")]
        [SerializeField]
        private int maxAlienLevel = 3;

        [SerializeField]
        private string alienUpgradeId = "debug-runner-upgrade";

        [SerializeField]
        private float alienUpgradeHealthMultiplier = 1.2f;

        [SerializeField]
        private float alienUpgradeSpeedMultiplier = 1.05f;

        [SerializeField]
        private float alienUpgradeRewardMultiplier = 1.15f;

        [SerializeField]
        private float alienUpgradeDodgeChanceBonus = 0.02f;

        [SerializeField]
        private DamageType alienUpgradeResistanceDamageType = DamageType.Kinetic;

        [SerializeField]
        private float alienUpgradeResistanceBonus = 0.05f;

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

        public int PerfectWaveScrapBonus
        {
            get { return perfectWaveScrapBonus; }
        }

        public int MaxTowerLevel
        {
            get { return maxTowerLevel; }
        }

        public int MaxAlienLevel
        {
            get { return System.Math.Max(1, maxAlienLevel); }
        }

        public string BasicAlienId
        {
            get { return basicAlienId; }
        }

        public string FastAlienId
        {
            get { return fastAlienId; }
        }

        public string ArmouredAlienId
        {
            get { return armouredAlienId; }
        }

        public WaveDefinition CreateWaveDefinition(int completedWaves)
        {
            if (completedWaves < 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(completedWaves), "Completed waves cannot be negative.");
            }

            var totalAliens = startingWaveAlienCount + completedWaves * extraAliensPerWave;
            var fastAliens = completedWaves <= 0 ? 0 : System.Math.Max(1, totalAliens / 4);
            var armouredAliens = completedWaves < 2 ? 0 : System.Math.Max(1, totalAliens / 5);
            var basicAliens = totalAliens - fastAliens - armouredAliens;
            var groups = new List<WaveSpawnGroup>();

            if (basicAliens > 0)
            {
                groups.Add(new WaveSpawnGroup(basicAlienId, basicAliens));
            }

            if (fastAliens > 0)
            {
                groups.Add(new WaveSpawnGroup(fastAlienId, fastAliens));
            }

            if (armouredAliens > 0)
            {
                groups.Add(new WaveSpawnGroup(armouredAlienId, armouredAliens));
            }

            return new WaveDefinition(
                new WaveComposition(groups),
                secondsBetweenSpawns,
                waveClearScrapReward);
        }

        public TowerDefinition CreateTowerDefinition()
        {
            return CreateTowerDefinitions()[0];
        }

        public IReadOnlyList<TowerDefinition> CreateTowerDefinitions()
        {
            return new[]
            {
                CreateRailgunTowerDefinition(),
                CreateMortarTowerDefinition()
            };
        }

        private TowerDefinition CreateRailgunTowerDefinition()
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
                towerCriticalDamageMultiplier,
                0,
                0,
                new[] { CreateTowerStatusEffectDefinition() });
        }

        private TowerDefinition CreateMortarTowerDefinition()
        {
            return new TowerDefinition(
                secondTowerId,
                secondTowerCost,
                secondTowerRange,
                secondTowerFireRatePerSecond,
                secondTowerDamage,
                secondTowerDamageType,
                secondTowerTargetingMode,
                secondTowerCriticalChance,
                secondTowerCriticalDamageMultiplier,
                secondTowerSplashRadius,
                secondTowerSplashDamageMultiplier);
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

        public PlayerAbilityDefinition CreateFreezePulseAbilityDefinition()
        {
            return new PlayerAbilityDefinition(
                freezePulseAbilityId,
                freezePulseCooldownSeconds,
                new AlienStatusEffectDefinition(
                    freezePulseStatusEffectId,
                    AlienStatusEffectType.Slow,
                    freezePulseDurationSeconds,
                    freezePulseMovementSpeedMultiplier));
        }

        public AlienStatusEffectDefinition CreateTowerStatusEffectDefinition()
        {
            return new AlienStatusEffectDefinition(
                towerStatusEffectId,
                AlienStatusEffectType.Slow,
                towerStatusEffectDurationSeconds,
                towerStatusEffectMovementSpeedMultiplier);
        }

        public AlienUpgradeDefinition CreateAlienUpgradeDefinition()
        {
            return new AlienUpgradeDefinition(
                alienUpgradeId,
                alienUpgradeHealthMultiplier,
                alienUpgradeSpeedMultiplier,
                alienUpgradeRewardMultiplier,
                alienUpgradeDodgeChanceBonus,
                alienUpgradeResistanceDamageType,
                alienUpgradeResistanceBonus);
        }

        public AlienDefinition CreateAlienDefinition()
        {
            return CreateAlienDefinition(0);
        }

        public AlienDefinition CreateAlienDefinition(int completedWaves)
        {
            return CreateAlienDefinition(basicAlienId, completedWaves);
        }

        public AlienDefinition CreateAlienDefinition(string alienId, int completedWaves)
        {
            if (completedWaves < 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(completedWaves), "Completed waves cannot be negative.");
            }

            var definition = CreateBaseAlienDefinition(alienId);
            var upgrade = CreateAlienUpgradeDefinition();
            var upgradeCount = System.Math.Min(completedWaves, MaxAlienLevel - 1);

            for (var index = 0; index < upgradeCount; index++)
            {
                definition = upgrade.ApplyTo(definition);
            }

            return definition;
        }

        private AlienDefinition CreateBaseAlienDefinition(string alienId)
        {
            if (alienId == basicAlienId)
            {
                return new AlienDefinition(
                    basicAlienId,
                    basicAlienHealth,
                    basicAlienSpeedCellsPerSecond,
                    basicAlienReward,
                    new Dictionary<DamageType, float>(),
                    basicAlienDodgeChance);
            }

            if (alienId == fastAlienId)
            {
                return new AlienDefinition(
                    fastAlienId,
                    fastAlienHealth,
                    fastAlienSpeedCellsPerSecond,
                    fastAlienReward,
                    new Dictionary<DamageType, float>(),
                    fastAlienDodgeChance);
            }

            if (alienId == armouredAlienId)
            {
                return new AlienDefinition(
                    armouredAlienId,
                    armouredAlienHealth,
                    armouredAlienSpeedCellsPerSecond,
                    armouredAlienReward,
                    new Dictionary<DamageType, float>
                    {
                        { armouredAlienResistanceDamageType, armouredAlienResistance }
                    },
                    armouredAlienDodgeChance);
            }

            throw new System.ArgumentException($"Unknown alien id '{alienId}'.", nameof(alienId));
        }
    }
}
