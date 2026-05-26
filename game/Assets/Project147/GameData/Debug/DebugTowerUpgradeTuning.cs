using System;
using Project147.GameCore.Combat;
using UnityEngine;

namespace Project147.GameData.Debug
{
    [Serializable]
    public sealed class DebugTowerUpgradeTuning
    {
        [SerializeField]
        private int maxTowerLevel = 3;

        [SerializeField]
        private string id = "debug-railgun-upgrade";

        [SerializeField]
        private int cost = 75;

        [SerializeField]
        private float damageMultiplier = 1.35f;

        [SerializeField]
        private float fireRateMultiplier = 1.15f;

        [SerializeField]
        private float rangeBonus = 0.2f;

        [SerializeField]
        private float criticalChanceBonus = 0.05f;

        [SerializeField]
        private float criticalDamageMultiplierBonus = 0.15f;

        [SerializeField]
        private float statusDurationMultiplier = 1;

        [SerializeField]
        private float statusDamageMultiplier = 1;

        [SerializeField]
        private float statusMovementSpeedMultiplier = 1;

        public DebugTowerUpgradeTuning()
        {
        }

        public DebugTowerUpgradeTuning(
            string id,
            int cost,
            float damageMultiplier,
            float fireRateMultiplier,
            float rangeBonus,
            float criticalChanceBonus,
            float criticalDamageMultiplierBonus,
            float statusDurationMultiplier = 1,
            float statusDamageMultiplier = 1,
            float statusMovementSpeedMultiplier = 1)
        {
            this.id = id;
            this.cost = cost;
            this.damageMultiplier = damageMultiplier;
            this.fireRateMultiplier = fireRateMultiplier;
            this.rangeBonus = rangeBonus;
            this.criticalChanceBonus = criticalChanceBonus;
            this.criticalDamageMultiplierBonus = criticalDamageMultiplierBonus;
            this.statusDurationMultiplier = statusDurationMultiplier;
            this.statusDamageMultiplier = statusDamageMultiplier;
            this.statusMovementSpeedMultiplier = statusMovementSpeedMultiplier;
        }

        public int MaxTowerLevel
        {
            get { return maxTowerLevel; }
        }

        public TowerUpgradeDefinition CreateDefinition()
        {
            return new TowerUpgradeDefinition(
                id,
                cost,
                damageMultiplier,
                fireRateMultiplier,
                rangeBonus,
                criticalChanceBonus,
                criticalDamageMultiplierBonus,
                statusDurationMultiplier,
                statusDamageMultiplier,
                statusMovementSpeedMultiplier);
        }
    }
}
