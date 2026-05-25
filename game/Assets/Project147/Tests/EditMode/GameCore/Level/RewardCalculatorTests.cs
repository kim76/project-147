using System;
using NUnit.Framework;
using Project147.GameCore.Combat;
using Project147.GameCore.Level;

namespace Project147.Tests.EditMode.GameCore.Level
{
    public sealed class RewardCalculatorTests
    {
        [Test]
        public void Constructor_WhenPerfectWaveBonusIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new RewardCalculator(-1));
        }

        [Test]
        public void CalculateAlienKillReward_WhenAlienIsNull_Throws()
        {
            var calculator = new RewardCalculator(10);

            Assert.Throws<ArgumentNullException>(() => calculator.CalculateAlienKillReward(null));
        }

        [Test]
        public void CalculateAlienKillReward_ReturnsAlienReward()
        {
            var calculator = new RewardCalculator(10);
            var alien = new AlienDefinition("runner-basic", 50, 1, 15, new System.Collections.Generic.Dictionary<DamageType, float>());

            var reward = calculator.CalculateAlienKillReward(alien);

            Assert.That(reward.Amount, Is.EqualTo(15));
            Assert.That(reward.Source, Is.EqualTo(RewardSource.AlienKill));
        }

        [Test]
        public void CalculateWaveClearReward_WhenWaveIsNull_Throws()
        {
            var calculator = new RewardCalculator(10);

            Assert.Throws<ArgumentNullException>(() => calculator.CalculateWaveClearReward(null, true));
        }

        [Test]
        public void CalculateWaveClearReward_WhenWaveIsNotPerfect_ReturnsClearReward()
        {
            var calculator = new RewardCalculator(10);
            var wave = new WaveDefinition(5, 0.5f, 25);

            var reward = calculator.CalculateWaveClearReward(wave, false);

            Assert.That(reward.Amount, Is.EqualTo(25));
            Assert.That(reward.Source, Is.EqualTo(RewardSource.WaveClear));
        }

        [Test]
        public void CalculateWaveClearReward_WhenWaveIsPerfect_IncludesPerfectBonus()
        {
            var calculator = new RewardCalculator(10);
            var wave = new WaveDefinition(5, 0.5f, 25);

            var reward = calculator.CalculateWaveClearReward(wave, true);

            Assert.That(reward.Amount, Is.EqualTo(35));
            Assert.That(reward.Source, Is.EqualTo(RewardSource.WaveClear));
        }
    }
}
