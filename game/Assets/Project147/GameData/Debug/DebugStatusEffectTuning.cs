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

        [SerializeField]
        private AlienStatusEffectType type;

        [SerializeField]
        private float damagePerSecond;

        public DebugStatusEffectTuning(
            string id,
            float durationSeconds,
            float movementSpeedMultiplier,
            AlienStatusEffectType type = AlienStatusEffectType.Slow,
            float damagePerSecond = 0)
        {
            this.id = id;
            this.durationSeconds = durationSeconds;
            this.movementSpeedMultiplier = movementSpeedMultiplier;
            this.type = type;
            this.damagePerSecond = damagePerSecond;
        }

        public AlienStatusEffectDefinition CreateDefinition()
        {
            return new AlienStatusEffectDefinition(
                id,
                type,
                durationSeconds,
                movementSpeedMultiplier,
                damagePerSecond);
        }
    }
}
