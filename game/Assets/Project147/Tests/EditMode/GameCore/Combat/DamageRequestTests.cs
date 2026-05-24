using System;
using NUnit.Framework;
using Project147.GameCore.Combat;

namespace Project147.Tests.EditMode.GameCore.Combat
{
    public sealed class DamageRequestTests
    {
        [Test]
        public void Constructor_WhenValuesAreValid_StoresValues()
        {
            var request = new DamageRequest(25, DamageType.Energy);

            Assert.That(request.Amount, Is.EqualTo(25));
            Assert.That(request.DamageType, Is.EqualTo(DamageType.Energy));
        }

        [Test]
        public void Constructor_WhenAmountIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DamageRequest(-1, DamageType.Energy));
        }
    }
}

