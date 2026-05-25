using System;
using Project147.GameCore.Combat;
using UnityEngine;

namespace Project147.GameData.Debug
{
    [Serializable]
    public sealed class DebugStatusEffectTuning
    {
        [SerializeField]
        private string id;

        [SerializeField]
        private float durationSeconds;

        [SerializeField]
        private float movementSpeedMultiplier;

        public DebugStatusEffectTuning(
            string id,
            float durationSeconds,
            float movementSpeedMultiplier)
        {
            this.id = id;
            this.durationSeconds = durationSeconds;
            this.movementSpeedMultiplier = movementSpeedMultiplier;
        }

        public AlienStatusEffectDefinition CreateDefinition()
        {
            return new AlienStatusEffectDefinition(
                id,
                AlienStatusEffectType.Slow,
                durationSeconds,
                movementSpeedMultiplier);
        }
    }
}
