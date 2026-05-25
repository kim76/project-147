using System;
using Project147.GameCore.Combat;

namespace Project147.GameCore.Level
{
    public sealed class RewardCalculator
    {
        public RewardCalculator(int perfectWaveBonus)
        {
            if (perfectWaveBonus < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(perfectWaveBonus), "Perfect wave bonus cannot be negative.");
            }

            PerfectWaveBonus = perfectWaveBonus;
        }

        public int PerfectWaveBonus { get; }

        public RewardResult CalculateAlienKillReward(AlienDefinition alien)
        {
            if (alien == null)
            {
                throw new ArgumentNullException(nameof(alien));
            }

            return new RewardResult(RewardSource.AlienKill, alien.Reward);
        }

        public RewardResult CalculateWaveClearReward(WaveDefinition wave, bool wasPerfectWave)
        {
            if (wave == null)
            {
                throw new ArgumentNullException(nameof(wave));
            }

            var amount = wave.ClearReward;

            if (wasPerfectWave)
            {
                amount += PerfectWaveBonus;
            }

            return new RewardResult(RewardSource.WaveClear, amount);
        }
    }
}
