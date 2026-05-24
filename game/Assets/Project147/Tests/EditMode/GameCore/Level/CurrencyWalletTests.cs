using System;
using NUnit.Framework;
using Project147.GameCore.Level;

namespace Project147.Tests.EditMode.GameCore.Level
{
    public sealed class CurrencyWalletTests
    {
        [Test]
        public void Constructor_WhenBalanceIsValid_StoresBalance()
        {
            var wallet = new CurrencyWallet(100);

            Assert.That(wallet.Balance, Is.EqualTo(100));
        }

        [Test]
        public void Constructor_WhenBalanceIsNegative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CurrencyWallet(-1));
        }

        [Test]
        public void Spend_WhenBalanceIsEnough_ReturnsWalletWithReducedBalance()
        {
            var wallet = new CurrencyWallet(100);

            var result = wallet.Spend(40);

            Assert.That(result.Balance, Is.EqualTo(60));
            Assert.That(wallet.Balance, Is.EqualTo(100));
        }

        [Test]
        public void Spend_WhenBalanceIsNotEnough_Throws()
        {
            var wallet = new CurrencyWallet(10);

            Assert.Throws<InvalidOperationException>(() => wallet.Spend(40));
        }

        [Test]
        public void Add_WhenAmountIsValid_ReturnsWalletWithIncreasedBalance()
        {
            var wallet = new CurrencyWallet(10);

            var result = wallet.Add(15);

            Assert.That(result.Balance, Is.EqualTo(25));
        }
    }
}

