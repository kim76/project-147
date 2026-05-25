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
                criticalDamageMultiplierBonus);
        }
    }
}
