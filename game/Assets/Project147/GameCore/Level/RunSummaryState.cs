using System;

namespace Project147.GameCore.Level
{
    public sealed class RunSummaryState
    {
        public RunSummaryState()
            : this(RunOutcome.InProgress, 0, 0, 0, 0, 0, 0, 0, 0, 0)
        {
        }

        private RunSummaryState(
            RunOutcome outcome,
            int aliensDestroyed,
            int aliensLeaked,
            int wavesCleared,
            int perfectWaves,
            int scrapEarned,
            int rewardsChosen,
            int freezePulseUses,
            int orbitalStrikeUses,
            int shieldBurstUses)
        {
            if (aliensDestroyed < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(aliensDestroyed), "Alien destroy count cannot be negative.");
            }

            if (aliensLeaked < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(aliensLeaked), "Alien leak count cannot be negative.");
            }

            if (wavesCleared < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(wavesCleared), "Wave clear count cannot be negative.");
            }

            if (perfectWaves < 0 || perfectWaves > wavesCleared)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(perfectWaves),
                    "Perfect wave count must be between zero and cleared waves.");
            }

            if (scrapEarned < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(scrapEarned), "Scrap earned cannot be negative.");
            }

            if (rewardsChosen < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rewardsChosen), "Reward choice count cannot be negative.");
            }

            if (freezePulseUses < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(freezePulseUses), "Freeze Pulse uses cannot be negative.");
            }

            if (orbitalStrikeUses < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(orbitalStrikeUses), "Orbital Strike uses cannot be negative.");
            }

            if (shieldBurstUses < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(shieldBurstUses), "Shield Burst uses cannot be negative.");
            }

            Outcome = outcome;
            AliensDestroyed = aliensDestroyed;
            AliensLeaked = aliensLeaked;
            WavesCleared = wavesCleared;
            PerfectWaves = perfectWaves;
            ScrapEarned = scrapEarned;
            RewardsChosen = rewardsChosen;
            FreezePulseUses = freezePulseUses;
            OrbitalStrikeUses = orbitalStrikeUses;
            ShieldBurstUses = shieldBurstUses;
        }

        public RunOutcome Outcome { get; }

        public int AliensDestroyed { get; }

        public int AliensLeaked { get; }

        public int WavesCleared { get; }

        public int PerfectWaves { get; }

        public int ScrapEarned { get; }

        public int RewardsChosen { get; }

        public int FreezePulseUses { get; }

        public int OrbitalStrikeUses { get; }

        public int ShieldBurstUses { get; }

        public int StarRating
        {
            get
            {
                if (Outcome != RunOutcome.Victory)
                {
                    return 0;
                }

                if (AliensLeaked == 0)
                {
                    return 3;
                }

                return AliensLeaked <= 2 ? 2 : 1;
            }
        }

        public RunSummaryState RecordAlienDestroyed(int scrapEarned)
        {
            return new RunSummaryState(
                Outcome,
                AliensDestroyed + 1,
                AliensLeaked,
                WavesCleared,
                PerfectWaves,
                AddScrap(scrapEarned),
                RewardsChosen,
                FreezePulseUses,
                OrbitalStrikeUses,
                ShieldBurstUses);
        }

        public RunSummaryState RecordAlienLeaked()
        {
            return new RunSummaryState(
                Outcome,
                AliensDestroyed,
                AliensLeaked + 1,
                WavesCleared,
                PerfectWaves,
                ScrapEarned,
                RewardsChosen,
                FreezePulseUses,
                OrbitalStrikeUses,
                ShieldBurstUses);
        }

        public RunSummaryState RecordWaveCleared(int scrapEarned, bool wasPerfectWave)
        {
            return new RunSummaryState(
                Outcome,
                AliensDestroyed,
                AliensLeaked,
                WavesCleared + 1,
                wasPerfectWave ? PerfectWaves + 1 : PerfectWaves,
                AddScrap(scrapEarned),
                RewardsChosen,
                FreezePulseUses,
                OrbitalStrikeUses,
                ShieldBurstUses);
        }

        public RunSummaryState RecordRewardChosen()
        {
            return new RunSummaryState(
                Outcome,
                AliensDestroyed,
                AliensLeaked,
                WavesCleared,
                PerfectWaves,
                ScrapEarned,
                RewardsChosen + 1,
                FreezePulseUses,
                OrbitalStrikeUses,
                ShieldBurstUses);
        }

        public RunSummaryState RecordFreezePulseUsed()
        {
            return new RunSummaryState(
                Outcome,
                AliensDestroyed,
                AliensLeaked,
                WavesCleared,
                PerfectWaves,
                ScrapEarned,
                RewardsChosen,
                FreezePulseUses + 1,
                OrbitalStrikeUses,
                ShieldBurstUses);
        }

        public RunSummaryState RecordOrbitalStrikeUsed()
        {
            return new RunSummaryState(
                Outcome,
                AliensDestroyed,
                AliensLeaked,
                WavesCleared,
                PerfectWaves,
                ScrapEarned,
                RewardsChosen,
                FreezePulseUses,
                OrbitalStrikeUses + 1,
                ShieldBurstUses);
        }

        public RunSummaryState RecordShieldBurstUsed()
        {
            return new RunSummaryState(
                Outcome,
                AliensDestroyed,
                AliensLeaked,
                WavesCleared,
                PerfectWaves,
                ScrapEarned,
                RewardsChosen,
                FreezePulseUses,
                OrbitalStrikeUses,
                ShieldBurstUses + 1);
        }

        public RunSummaryState Complete(RunOutcome outcome)
        {
            if (outcome == RunOutcome.InProgress)
            {
                throw new ArgumentException("Completed run outcome must be victory or defeat.", nameof(outcome));
            }

            return new RunSummaryState(
                outcome,
                AliensDestroyed,
                AliensLeaked,
                WavesCleared,
                PerfectWaves,
                ScrapEarned,
                RewardsChosen,
                FreezePulseUses,
                OrbitalStrikeUses,
                ShieldBurstUses);
        }

        private int AddScrap(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Scrap earned cannot be negative.");
            }

            return ScrapEarned + amount;
        }
    }
}
