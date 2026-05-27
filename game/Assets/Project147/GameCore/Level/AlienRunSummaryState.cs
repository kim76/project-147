using System;

namespace Project147.GameCore.Level
{
    public sealed class AlienRunSummaryState
    {
        public AlienRunSummaryState(
            AlienRunOutcome outcome,
            string squadPlanId,
            string upgradePlanId,
            int squadSize,
            int aliensLeaked,
            int aliensDestroyed,
            int startingBaseHealth,
            int remainingBaseHealth,
            int defenceTowerCount)
        {
            if (string.IsNullOrWhiteSpace(squadPlanId))
            {
                throw new ArgumentException("Alien run squad plan id is required.", nameof(squadPlanId));
            }

            if (string.IsNullOrWhiteSpace(upgradePlanId))
            {
                throw new ArgumentException("Alien run upgrade plan id is required.", nameof(upgradePlanId));
            }

            if (squadSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(squadSize), "Alien run squad size must be greater than zero.");
            }

            if (aliensLeaked < 0 || aliensLeaked > squadSize)
            {
                throw new ArgumentOutOfRangeException(nameof(aliensLeaked), "Alien leaks must be between zero and squad size.");
            }

            if (aliensDestroyed < 0 || aliensDestroyed > squadSize)
            {
                throw new ArgumentOutOfRangeException(nameof(aliensDestroyed), "Destroyed aliens must be between zero and squad size.");
            }

            if (startingBaseHealth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startingBaseHealth), "Starting base health must be greater than zero.");
            }

            if (remainingBaseHealth < 0 || remainingBaseHealth > startingBaseHealth)
            {
                throw new ArgumentOutOfRangeException(nameof(remainingBaseHealth), "Remaining base health must be between zero and starting health.");
            }

            if (defenceTowerCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(defenceTowerCount), "Defence tower count cannot be negative.");
            }

            Outcome = outcome;
            SquadPlanId = squadPlanId;
            UpgradePlanId = upgradePlanId;
            SquadSize = squadSize;
            AliensLeaked = aliensLeaked;
            AliensDestroyed = aliensDestroyed;
            StartingBaseHealth = startingBaseHealth;
            RemainingBaseHealth = remainingBaseHealth;
            DefenceTowerCount = defenceTowerCount;
        }

        public AlienRunOutcome Outcome { get; }

        public string SquadPlanId { get; }

        public string UpgradePlanId { get; }

        public int SquadSize { get; }

        public int AliensLeaked { get; }

        public int AliensDestroyed { get; }

        public int StartingBaseHealth { get; }

        public int RemainingBaseHealth { get; }

        public int DefenceTowerCount { get; }

        public int BaseDamageDealt
        {
            get { return StartingBaseHealth - RemainingBaseHealth; }
        }

        public int AliensStopped
        {
            get { return SquadSize - AliensLeaked; }
        }

        public int Score
        {
            get { return BaseDamageDealt * 100 + AliensLeaked * 50 + (Outcome == AlienRunOutcome.AlienVictory ? 500 : 0); }
        }

        public int StarRating
        {
            get
            {
                if (Outcome == AlienRunOutcome.AlienVictory)
                {
                    return 3;
                }

                if (BaseDamageDealt >= StartingBaseHealth / 2)
                {
                    return 2;
                }

                return BaseDamageDealt > 0 ? 1 : 0;
            }
        }
    }
}
