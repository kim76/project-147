using System;
using System.Collections.Generic;
using Project147.GameCore.Abilities;
using Project147.GameCore.Choices;
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
        private DebugLevelTuning level = new DebugLevelTuning();

        [Header("Towers")]
        [SerializeField]
        private DebugTowerTuning railgunTower = new DebugTowerTuning(
            "debug-railgun",
            50,
            2.35f,
            1.25f,
            24,
            DamageType.Kinetic,
            TowerTargetingMode.First,
            0,
            1.5f,
            0,
            0);

        [SerializeField]
        private DebugTowerTuning mortarTower = new DebugTowerTuning(
            "debug-mortar",
            70,
            2.8f,
            0.75f,
            38,
            DamageType.Explosive,
            TowerTargetingMode.Strongest,
            0.1f,
            1.8f,
            1.25f,
            0.45f);

        [SerializeField]
        private DebugTowerTuning energyTower = CreateDefaultEnergyTowerTuning();

        [SerializeField]
        private DebugTowerUpgradeTuning towerUpgrade = new DebugTowerUpgradeTuning();

        [SerializeField]
        private DebugStatusEffectTuning railgunSlow = new DebugStatusEffectTuning(
            "debug-frost-slow",
            1.4f,
            0.7f);

        [Header("Player Abilities")]
        [SerializeField]
        private DebugAbilityTuning abilities = new DebugAbilityTuning();

        [Header("Run Choices")]
        [SerializeField]
        private DebugRunChoiceTuning runChoices = new DebugRunChoiceTuning();

        [Header("Aliens")]
        [SerializeField]
        private DebugAlienTuning basicAlien = new DebugAlienTuning(
            "debug-basic",
            60,
            1.45f,
            15,
            0,
            DamageType.Kinetic,
            0);

        [SerializeField]
        private DebugAlienTuning fastAlien = new DebugAlienTuning(
            "debug-fast",
            38,
            2.25f,
            12,
            0.12f,
            DamageType.Kinetic,
            0);

        [SerializeField]
        private DebugAlienTuning armouredAlien = new DebugAlienTuning(
            "debug-armoured",
            115,
            0.95f,
            26,
            0,
            DamageType.Kinetic,
            0.35f);

        [SerializeField]
        private DebugAlienTuning bossAlien = CreateDefaultBossAlienTuning();

        [SerializeField]
        private DebugAlienTuning shieldedAlien = CreateDefaultShieldedAlienTuning();

        [SerializeField]
        private DebugAlienUpgradeTuning alienUpgrade = new DebugAlienUpgradeTuning();

        public int StartingCurrency
        {
            get { return level.StartingCurrency; }
        }

        public int BaseHealth
        {
            get { return level.BaseHealth; }
        }

        public int TotalWaves
        {
            get { return level.TotalWaves; }
        }

        public int PerfectWaveScrapBonus
        {
            get { return level.PerfectWaveScrapBonus; }
        }

        public int MaxTowerLevel
        {
            get { return towerUpgrade.MaxTowerLevel; }
        }

        public int MaxAlienLevel
        {
            get { return alienUpgrade.MaxAlienLevel; }
        }

        public string BasicAlienId
        {
            get { return basicAlien.Id; }
        }

        public string FastAlienId
        {
            get { return fastAlien.Id; }
        }

        public string ArmouredAlienId
        {
            get { return armouredAlien.Id; }
        }

        public string BossAlienId
        {
            get { return BossAlien.Id; }
        }

        public string ShieldedAlienId
        {
            get { return ShieldedAlien.Id; }
        }

        public WaveDefinition CreateWaveDefinition(int completedWaves)
        {
            return level.CreateWaveDefinition(
                completedWaves,
                BasicAlienId,
                FastAlienId,
                ArmouredAlienId,
                ShieldedAlienId,
                BossAlienId);
        }

        public TowerDefinition CreateTowerDefinition()
        {
            return CreateTowerDefinitions()[0];
        }

        public IReadOnlyList<TowerDefinition> CreateTowerDefinitions()
        {
            return new[]
            {
                railgunTower.CreateDefinition(new[] { CreateTowerStatusEffectDefinition() }),
                mortarTower.CreateDefinition(),
                EnergyTower.CreateDefinition()
            };
        }

        public TowerUpgradeDefinition CreateTowerUpgradeDefinition()
        {
            return towerUpgrade.CreateDefinition();
        }

        public PlayerAbilityDefinition CreateFreezePulseAbilityDefinition()
        {
            return abilities.CreateFreezePulseDefinition();
        }

        public PlayerAbilityDefinition CreateOrbitalStrikeAbilityDefinition()
        {
            return abilities.CreateOrbitalStrikeDefinition();
        }

        public IReadOnlyList<RunChoiceDefinition> CreateRunChoiceDefinitions()
        {
            return runChoices.CreateDefinitions();
        }

        public AlienStatusEffectDefinition CreateTowerStatusEffectDefinition()
        {
            return railgunSlow.CreateDefinition();
        }

        public AlienUpgradeDefinition CreateAlienUpgradeDefinition()
        {
            return alienUpgrade.CreateDefinition();
        }

        public AlienDefinition CreateAlienDefinition()
        {
            return CreateAlienDefinition(0);
        }

        public AlienDefinition CreateAlienDefinition(int completedWaves)
        {
            return CreateAlienDefinition(BasicAlienId, completedWaves);
        }

        public AlienDefinition CreateAlienDefinition(string alienId, int completedWaves)
        {
            if (completedWaves < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(completedWaves), "Completed waves cannot be negative.");
            }

            var definition = CreateBaseAlienDefinition(alienId);
            var upgrade = CreateAlienUpgradeDefinition();
            var upgradeCount = Math.Min(completedWaves, MaxAlienLevel - 1);

            for (var index = 0; index < upgradeCount; index++)
            {
                definition = upgrade.ApplyTo(definition);
            }

            return definition;
        }

        private AlienDefinition CreateBaseAlienDefinition(string alienId)
        {
            if (alienId == BasicAlienId)
            {
                return basicAlien.CreateDefinition();
            }

            if (alienId == FastAlienId)
            {
                return fastAlien.CreateDefinition();
            }

            if (alienId == ArmouredAlienId)
            {
                return armouredAlien.CreateDefinition();
            }

            if (alienId == ShieldedAlienId)
            {
                return ShieldedAlien.CreateDefinition();
            }

            if (alienId == BossAlienId)
            {
                return BossAlien.CreateDefinition();
            }

            throw new ArgumentException($"Unknown alien id '{alienId}'.", nameof(alienId));
        }

        private DebugAlienTuning BossAlien
        {
            get { return bossAlien ?? CreateDefaultBossAlienTuning(); }
        }

        private DebugAlienTuning ShieldedAlien
        {
            get { return shieldedAlien ?? CreateDefaultShieldedAlienTuning(); }
        }

        private DebugTowerTuning EnergyTower
        {
            get { return energyTower ?? CreateDefaultEnergyTowerTuning(); }
        }

        private static DebugTowerTuning CreateDefaultEnergyTowerTuning()
        {
            return new DebugTowerTuning(
                "debug-energy",
                65,
                2.45f,
                1f,
                30,
                DamageType.Energy,
                TowerTargetingMode.Closest,
                0.05f,
                1.6f,
                0,
                0);
        }

        private static DebugAlienTuning CreateDefaultShieldedAlienTuning()
        {
            return new DebugAlienTuning(
                "debug-shielded",
                80,
                1.05f,
                24,
                0.02f,
                DamageType.Kinetic,
                0.1f,
                65);
        }

        private static DebugAlienTuning CreateDefaultBossAlienTuning()
        {
            return new DebugAlienTuning(
                "debug-boss",
                320,
                0.72f,
                90,
                0.05f,
                DamageType.Explosive,
                0.25f);
        }
    }
}
