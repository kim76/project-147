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

        [Header("Layouts")]
        [SerializeField]
        private DebugLevelLayoutTuning relayYardLayout = CreateRelayYardLayout();

        [SerializeField]
        private DebugLevelLayoutTuning switchbackLayout = CreateSwitchbackLayout();

        [SerializeField]
        private DebugLevelLayoutTuning twinLaneLayout = CreateTwinLaneLayout();

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
        private DebugTowerTuning chemicalTower = CreateDefaultChemicalTowerTuning();

        [SerializeField]
        private DebugTowerUpgradeTuning towerUpgrade = new DebugTowerUpgradeTuning();

        [SerializeField]
        private DebugTowerUpgradeTuning damageTowerUpgrade = new DebugTowerUpgradeTuning(
            "debug-upgrade-damage",
            75,
            1.55f,
            1f,
            0,
            0.03f,
            0.1f);

        [SerializeField]
        private DebugTowerUpgradeTuning rapidTowerUpgrade = new DebugTowerUpgradeTuning(
            "debug-upgrade-rapid",
            75,
            1.08f,
            1.35f,
            0,
            0.03f,
            0.05f);

        [SerializeField]
        private DebugTowerUpgradeTuning rangeTowerUpgrade = new DebugTowerUpgradeTuning(
            "debug-upgrade-range",
            75,
            1.12f,
            1.05f,
            0.45f,
            0.08f,
            0.2f);

        [SerializeField]
        private DebugTowerUpgradeTuning statusTowerUpgrade = new DebugTowerUpgradeTuning(
            "debug-upgrade-status",
            75,
            1.05f,
            1f,
            0.1f,
            0.02f,
            0.05f,
            1.35f,
            1.35f,
            0.85f);

        [SerializeField]
        private DebugStatusEffectTuning railgunSlow = new DebugStatusEffectTuning(
            "debug-frost-slow",
            1.4f,
            0.7f);

        [SerializeField]
        private DebugStatusEffectTuning chemicalPoison = CreateDefaultChemicalPoisonTuning();

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
        private DebugAlienTuning burrowerAlien = CreateDefaultBurrowerAlienTuning();

        [SerializeField]
        private DebugAlienTuning regeneratorAlien = CreateDefaultRegeneratorAlienTuning();

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

        public string BurrowerAlienId
        {
            get { return BurrowerAlien.Id; }
        }

        public string RegeneratorAlienId
        {
            get { return RegeneratorAlien.Id; }
        }

        public WaveDefinition CreateWaveDefinition(int completedWaves)
        {
            return CreateWaveDefinition(completedWaves, TotalWaves);
        }

        public WaveDefinition CreateWaveDefinition(int completedWaves, int totalWaves)
        {
            return level.CreateWaveDefinition(
                completedWaves,
                totalWaves,
                BasicAlienId,
                FastAlienId,
                ArmouredAlienId,
                ShieldedAlienId,
                BurrowerAlienId,
                RegeneratorAlienId,
                BossAlienId);
        }

        public LevelLayoutDefinition CreateLevelLayout()
        {
            return CreateLevelLayouts()[0];
        }

        public IReadOnlyList<LevelLayoutDefinition> CreateLevelLayouts()
        {
            return new[]
            {
                RelayYardLayout.CreateDefinition(),
                SwitchbackLayout.CreateDefinition(),
                TwinLaneLayout.CreateDefinition()
            };
        }

        public IReadOnlyList<LevelRunDefinition> CreateLevelDefinitions()
        {
            var layouts = CreateLevelLayouts();

            return new[]
            {
                new LevelRunDefinition(
                    layouts[0],
                    StartingCurrency,
                    BaseHealth,
                    TotalWaves,
                    PerfectWaveScrapBonus),
                new LevelRunDefinition(
                    layouts[1],
                    StartingCurrency,
                    BaseHealth,
                    TotalWaves + 1,
                    PerfectWaveScrapBonus + 5),
                new LevelRunDefinition(
                    layouts[2],
                    StartingCurrency + 40,
                    Math.Max(1, BaseHealth - 2),
                    TotalWaves,
                    PerfectWaveScrapBonus)
            };
        }

        public IReadOnlyList<LevelUnlockRule> CreateLevelUnlockRules()
        {
            var levels = CreateLevelDefinitions();

            return new[]
            {
                new LevelUnlockRule(levels[0].Layout.Id, 0),
                new LevelUnlockRule(levels[1].Layout.Id, 2),
                new LevelUnlockRule(levels[2].Layout.Id, 4)
            };
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
                EnergyTower.CreateDefinition(),
                ChemicalTower.CreateDefinition(new[] { CreateChemicalStatusEffectDefinition() })
            };
        }

        public IReadOnlyList<TowerLoadoutPlan> CreateTowerLoadoutPlans()
        {
            var towers = CreateTowerDefinitions();
            var railgun = towers[0];
            var mortar = towers[1];
            var energy = towers[2];
            var chemical = towers[3];

            return new[]
            {
                new TowerLoadoutPlan("debug-loadout-balanced", new[] { railgun, mortar, energy }),
                new TowerLoadoutPlan("debug-loadout-status", new[] { railgun, chemical, energy }),
                new TowerLoadoutPlan("debug-loadout-heavy", new[] { mortar, chemical, railgun })
            };
        }

        public IReadOnlyList<TowerLoadoutPlan> CreateTowerLoadoutPlans(TowerUnlockState unlockState)
        {
            if (unlockState == null)
            {
                throw new ArgumentNullException(nameof(unlockState));
            }

            var plans = new List<TowerLoadoutPlan>();

            foreach (var plan in CreateTowerLoadoutPlans())
            {
                plans.Add(unlockState.FilterPlan(plan));
            }

            return plans;
        }

        public TowerUnlockState CreateInitialTowerUnlockState()
        {
            var towerIds = new List<string>();

            foreach (var tower in CreateTowerDefinitions())
            {
                towerIds.Add(tower.Id);
            }

            return new TowerUnlockState(towerIds);
        }

        public TowerUpgradeDefinition CreateTowerUpgradeDefinition()
        {
            return CreateTowerUpgradeDefinitions()[0];
        }

        public IReadOnlyList<TowerUpgradeDefinition> CreateTowerUpgradeDefinitions()
        {
            return new[]
            {
                DamageTowerUpgrade.CreateDefinition(),
                RapidTowerUpgrade.CreateDefinition(),
                RangeTowerUpgrade.CreateDefinition(),
                StatusTowerUpgrade.CreateDefinition()
            };
        }

        public PlayerAbilityDefinition CreateFreezePulseAbilityDefinition()
        {
            return abilities.CreateFreezePulseDefinition();
        }

        public PlayerAbilityDefinition CreateOrbitalStrikeAbilityDefinition()
        {
            return abilities.CreateOrbitalStrikeDefinition();
        }

        public PlayerAbilityDefinition CreateShieldBurstAbilityDefinition()
        {
            return abilities.CreateShieldBurstDefinition();
        }

        public PlayerAbilityDefinition CreateTowerOverchargeAbilityDefinition()
        {
            return abilities.CreateTowerOverchargeDefinition();
        }

        public IReadOnlyList<RunChoiceDefinition> CreateRunChoiceDefinitions()
        {
            return runChoices.CreateDefinitions();
        }

        public AlienStatusEffectDefinition CreateTowerStatusEffectDefinition()
        {
            return railgunSlow.CreateDefinition();
        }

        public AlienStatusEffectDefinition CreateChemicalStatusEffectDefinition()
        {
            return ChemicalPoison.CreateDefinition();
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

            if (alienId == BurrowerAlienId)
            {
                return BurrowerAlien.CreateDefinition();
            }

            if (alienId == RegeneratorAlienId)
            {
                return RegeneratorAlien.CreateDefinition();
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

        private DebugLevelLayoutTuning RelayYardLayout
        {
            get { return relayYardLayout ?? CreateRelayYardLayout(); }
        }

        private DebugLevelLayoutTuning SwitchbackLayout
        {
            get { return switchbackLayout ?? CreateSwitchbackLayout(); }
        }

        private DebugLevelLayoutTuning TwinLaneLayout
        {
            get { return twinLaneLayout ?? CreateTwinLaneLayout(); }
        }

        private DebugTowerUpgradeTuning DamageTowerUpgrade
        {
            get { return damageTowerUpgrade ?? towerUpgrade ?? new DebugTowerUpgradeTuning(); }
        }

        private DebugTowerUpgradeTuning RapidTowerUpgrade
        {
            get { return rapidTowerUpgrade ?? towerUpgrade ?? new DebugTowerUpgradeTuning(); }
        }

        private DebugTowerUpgradeTuning RangeTowerUpgrade
        {
            get { return rangeTowerUpgrade ?? towerUpgrade ?? new DebugTowerUpgradeTuning(); }
        }

        private DebugTowerUpgradeTuning StatusTowerUpgrade
        {
            get { return statusTowerUpgrade ?? towerUpgrade ?? new DebugTowerUpgradeTuning(); }
        }

        private DebugTowerTuning ChemicalTower
        {
            get { return chemicalTower ?? CreateDefaultChemicalTowerTuning(); }
        }

        private DebugStatusEffectTuning ChemicalPoison
        {
            get { return chemicalPoison ?? CreateDefaultChemicalPoisonTuning(); }
        }

        private DebugAlienTuning BurrowerAlien
        {
            get { return burrowerAlien ?? CreateDefaultBurrowerAlienTuning(); }
        }

        private DebugAlienTuning RegeneratorAlien
        {
            get { return regeneratorAlien ?? CreateDefaultRegeneratorAlienTuning(); }
        }

        private static DebugLevelLayoutTuning CreateRelayYardLayout()
        {
            return new DebugLevelLayoutTuning(
                "debug-relay-yard",
                8,
                5,
                new Vector2Int(0, 2),
                new Vector2Int(7, 2),
                new[]
                {
                    new Vector2Int(2, 1),
                    new Vector2Int(2, 2),
                    new Vector2Int(2, 3),
                    new Vector2Int(5, 1),
                    new Vector2Int(5, 2),
                    new Vector2Int(5, 3)
                });
        }

        private static DebugLevelLayoutTuning CreateSwitchbackLayout()
        {
            return new DebugLevelLayoutTuning(
                "debug-switchback",
                9,
                6,
                new Vector2Int(0, 1),
                new Vector2Int(8, 4),
                new[]
                {
                    new Vector2Int(2, 0),
                    new Vector2Int(2, 1),
                    new Vector2Int(2, 2),
                    new Vector2Int(4, 3),
                    new Vector2Int(4, 4),
                    new Vector2Int(4, 5),
                    new Vector2Int(6, 0),
                    new Vector2Int(6, 1),
                    new Vector2Int(6, 2)
                });
        }

        private static DebugLevelLayoutTuning CreateTwinLaneLayout()
        {
            return new DebugLevelLayoutTuning(
                "debug-twin-lane",
                9,
                5,
                new Vector2Int(0, 2),
                new Vector2Int(8, 2),
                new[]
                {
                    new Vector2Int(1, 1),
                    new Vector2Int(1, 3),
                    new Vector2Int(3, 0),
                    new Vector2Int(3, 1),
                    new Vector2Int(3, 3),
                    new Vector2Int(3, 4),
                    new Vector2Int(5, 1),
                    new Vector2Int(5, 3),
                    new Vector2Int(7, 1),
                    new Vector2Int(7, 3)
                });
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

        private static DebugTowerTuning CreateDefaultChemicalTowerTuning()
        {
            return new DebugTowerTuning(
                "debug-chemical",
                60,
                2.1f,
                1.05f,
                16,
                DamageType.Chemical,
                TowerTargetingMode.Weakest,
                0.05f,
                1.5f,
                0,
                0);
        }

        private static DebugStatusEffectTuning CreateDefaultChemicalPoisonTuning()
        {
            return new DebugStatusEffectTuning(
                "debug-acid-burn",
                3f,
                1f,
                AlienStatusEffectType.Poison,
                6f);
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

        private static DebugAlienTuning CreateDefaultBurrowerAlienTuning()
        {
            return new DebugAlienTuning(
                "debug-burrower",
                52,
                1.65f,
                22,
                0.08f,
                DamageType.Explosive,
                0.1f,
                0,
                3);
        }

        private static DebugAlienTuning CreateDefaultRegeneratorAlienTuning()
        {
            return new DebugAlienTuning(
                "debug-regenerator",
                88,
                1.15f,
                28,
                0.02f,
                DamageType.Chemical,
                0.2f,
                0,
                0,
                5);
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
