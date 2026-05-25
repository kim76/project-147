using System;
using Project147.GameCore.Abilities;
using Project147.GameCore.Combat;
using UnityEngine;

namespace Project147.GameData.Debug
{
    [Serializable]
    public sealed class DebugAbilityTuning
    {
        [SerializeField]
        private string id = "debug-freeze-pulse";

        [SerializeField]
        private float cooldownSeconds = 12;

        [SerializeField]
        private string statusEffectId = "debug-freeze-pulse-slow";

        [SerializeField]
        private float durationSeconds = 2.2f;

        [SerializeField]
        private float movementSpeedMultiplier = 0.35f;

        [SerializeField]
        private string orbitalStrikeId = "debug-orbital-strike";

        [SerializeField]
        private float orbitalStrikeCooldownSeconds = 18;

        [SerializeField]
        private float orbitalStrikeDamage = 35;

        [SerializeField]
        private DamageType orbitalStrikeDamageType = DamageType.Energy;

        public PlayerAbilityDefinition CreateFreezePulseDefinition()
        {
            return new PlayerAbilityDefinition(
                id,
                cooldownSeconds,
                new AlienStatusEffectDefinition(
                    statusEffectId,
                    AlienStatusEffectType.Slow,
                    durationSeconds,
                    movementSpeedMultiplier));
        }

        public PlayerAbilityDefinition CreateOrbitalStrikeDefinition()
        {
            return new PlayerAbilityDefinition(
                orbitalStrikeId,
                orbitalStrikeCooldownSeconds,
                orbitalStrikeDamage,
                orbitalStrikeDamageType);
        }
    }
}
