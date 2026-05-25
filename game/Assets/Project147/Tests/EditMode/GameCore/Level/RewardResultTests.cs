using System;
using NUnit.Framework;
using Project147.GameCore.Level;

namespace Project147.Tests.EditMode.GameCore.Level
{
    public sealed class RewardResultTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_StoresValues()
        {
            var result = new RewardResult(RewardSource.WaveClear, 25);

            Assert.That(result.Source, Is.EqualTo(RewardSource.WaveClear));
            Assert.That(result.Amount, Is.EqualTo(25));
        }

        [Test]
        public void Constructor_WhenAmountIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new RewardResult(RewardSource.AlienKill, -1));
        }
    }
}
