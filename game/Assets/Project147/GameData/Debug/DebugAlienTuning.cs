using System;
using System.Collections.Generic;
using Project147.GameCore.Combat;
using UnityEngine;

namespace Project147.GameData.Debug
{
    [Serializable]
    public sealed class DebugAlienTuning
    {
        [SerializeField]
        private string id;

        [SerializeField]
        private float health;

        [SerializeField]
        private float speedCellsPerSecond;

        [SerializeField]
        private int reward;

        [SerializeField]
        private float dodgeChance;

        [SerializeField]
        private DamageType resistanceDamageType;

        [SerializeField]
        private float resistance;

        [SerializeField]
        private float shieldCapacity;

        public DebugAlienTuning(
            string id,
            float health,
            float speedCellsPerSecond,
            int reward,
            float dodgeChance,
            DamageType resistanceDamageType,
            float resistance,
            float shieldCapacity = 0)
        {
            this.id = id;
            this.health = health;
            this.speedCellsPerSecond = speedCellsPerSecond;
            this.reward = reward;
            this.dodgeChance = dodgeChance;
            this.resistanceDamageType = resistanceDamageType;
            this.resistance = resistance;
            this.shieldCapacity = shieldCapacity;
        }

        public string Id
        {
            get { return id; }
        }

        public AlienDefinition CreateDefinition()
        {
            var resistances = new Dictionary<DamageType, float>();

            if (resistance > 0)
            {
                resistances.Add(resistanceDamageType, resistance);
            }

            return new AlienDefinition(
                id,
                health,
                speedCellsPerSecond,
                reward,
                resistances,
                dodgeChance,
                shieldCapacity);
        }
    }
}
