using System;
using System.Collections.Generic;
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

        public IReadOnlyList<AlienUpgradeChoiceDefinition> CreateChoiceDefinitions()
        {
            return new[]
            {
                new AlienUpgradeChoiceDefinition(
                    "debug-alien-choice-health",
                    "Hardened Carapaces",
                    3,
                    new AlienUpgradeDefinition("debug-alien-upgrade-health", 1.25f, 1, 1.1f, 0, DamageType.Kinetic, 0)),
                new AlienUpgradeChoiceDefinition(
                    "debug-alien-choice-speed",
                    "Skitter Drives",
                    2,
                    new AlienUpgradeDefinition("debug-alien-upgrade-speed", 1, 1.15f, 1.05f, 0, DamageType.Kinetic, 0)),
                new AlienUpgradeChoiceDefinition(
                    "debug-alien-choice-dodge",
                    "Evasive Pattern",
                    2,
                    new AlienUpgradeDefinition("debug-alien-upgrade-dodge", 1, 1, 1.05f, 0.08f, DamageType.Kinetic, 0)),
                new AlienUpgradeChoiceDefinition(
                    "debug-alien-choice-resistance",
                    "Kinetic Plating",
                    3,
                    new AlienUpgradeDefinition("debug-alien-upgrade-resistance", 1, 1, 1.05f, 0, DamageType.Kinetic, 0.15f)),
                new AlienUpgradeChoiceDefinition(
                    "debug-alien-choice-shield",
                    "Shield Spores",
                    3,
                    new AlienUpgradeDefinition("debug-alien-upgrade-shield", 1, 1, 1.1f, 0, DamageType.Energy, 0, 35, 0)),
                new AlienUpgradeChoiceDefinition(
                    "debug-alien-choice-regeneration",
                    "Regeneration Glands",
                    3,
                    new AlienUpgradeDefinition("debug-alien-upgrade-regeneration", 1, 1, 1.1f, 0, DamageType.Chemical, 0, 0, 4))
            };
        }
    }
}
