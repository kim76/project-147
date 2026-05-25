using System;
using Project147.GameCore.Combat;
using UnityEngine;

namespace Project147.GameData.Debug
{
    [Serializable]
    public sealed class DebugAlienUpgradeTuning
    {
        [SerializeField]
        private int maxAlienLevel = 3;

        [SerializeField]
        private string id = "debug-runner-upgrade";

        [SerializeField]
        private float healthMultiplier = 1.2f;

        [SerializeField]
        private float speedMultiplier = 1.05f;

        [SerializeField]
        private float rewardMultiplier = 1.15f;

        [SerializeField]
        private float dodgeChanceBonus = 0.02f;

        [SerializeField]
        private DamageType resistanceDamageType = DamageType.Kinetic;

        [SerializeField]
        private float resistanceBonus = 0.05f;

        public int MaxAlienLevel
        {
            get { return Math.Max(1, maxAlienLevel); }
        }

        public AlienUpgradeDefinition CreateDefinition()
        {
            return new AlienUpgradeDefinition(
                id,
                healthMultiplier,
                speedMultiplier,
                rewardMultiplier,
                dodgeChanceBonus,
                resistanceDamageType,
                resistanceBonus);
        }
    }
}
