using System;
using System.Collections.Generic;
using Project147.GameCore.Choices;
using UnityEngine;

namespace Project147.GameData.Debug
{
    [Serializable]
    public sealed class DebugRunChoiceTuning
    {
        [SerializeField]
        private int salvageScrap = 45;

        [SerializeField]
        private int emergencyRepair = 2;

        [SerializeField]
        private int bigSalvageScrap = 75;

        [SerializeField]
        private int nextTowerDiscount = 30;

        [SerializeField]
        private int nextWaveTowerDamagePercent = 25;

        public IReadOnlyList<RunChoiceDefinition> CreateDefinitions()
        {
            return new[]
            {
                new RunChoiceDefinition(
                    "salvage-drop",
                    "Salvage Drop",
                    RunChoiceEffectType.AddScrap,
                    salvageScrap),
                new RunChoiceDefinition(
                    "field-repair",
                    "Field Repair",
                    RunChoiceEffectType.RepairBase,
                    emergencyRepair),
                new RunChoiceDefinition(
                    "scrap-windfall",
                    "Scrap Windfall",
                    RunChoiceEffectType.AddScrap,
                    bigSalvageScrap),
                new RunChoiceDefinition(
                    "construction-credit",
                    "Construction Credit",
                    RunChoiceEffectType.AddNextTowerDiscount,
                    nextTowerDiscount),
                new RunChoiceDefinition(
                    "overclock",
                    "Overclock",
                    RunChoiceEffectType.AddNextWaveTowerDamagePercent,
                    nextWaveTowerDamagePercent)
            };
        }
    }
}
