using System;
using System.Collections.Generic;
using Project147.GameCore.Combat;
using UnityEngine;

namespace Project147.GameData.Debug
{
    [Serializable]
    public sealed class DebugTowerTuning
    {
        [SerializeField]
        private string id;

        [SerializeField]
        private int cost;

        [SerializeField]
        private float range;

        [SerializeField]
        private float fireRatePerSecond;

        [SerializeField]
        private float damage;

        [SerializeField]
        private DamageType damageType;

        [SerializeField]
        private TowerTargetingMode targetingMode;

        [SerializeField]
        private float criticalChance;

        [SerializeField]
        private float criticalDamageMultiplier = 1;

        [SerializeField]
        private float splashRadius;

        [SerializeField]
        private float splashDamageMultiplier;

        public DebugTowerTuning(
            string id,
            int cost,
            float range,
            float fireRatePerSecond,
            float damage,
            DamageType damageType,
            TowerTargetingMode targetingMode,
            float criticalChance,
            float criticalDamageMultiplier,
            float splashRadius,
            float splashDamageMultiplier)
        {
            this.id = id;
            this.cost = cost;
            this.range = range;
            this.fireRatePerSecond = fireRatePerSecond;
            this.damage = damage;
            this.damageType = damageType;
            this.targetingMode = targetingMode;
            this.criticalChance = criticalChance;
            this.criticalDamageMultiplier = criticalDamageMultiplier;
            this.splashRadius = splashRadius;
            this.splashDamageMultiplier = splashDamageMultiplier;
        }

        public TowerDefinition CreateDefinition(
            IReadOnlyList<AlienStatusEffectDefinition> statusEffects = null)
        {
            return new TowerDefinition(
                id,
                cost,
                range,
                fireRatePerSecond,
                damage,
                damageType,
                targetingMode,
                criticalChance,
                criticalDamageMultiplier,
                splashRadius,
                splashDamageMultiplier,
                statusEffects);
        }
    }
}
