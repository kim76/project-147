using System;
using NUnit.Framework;
using Project147.GameCore.Choices;
using Project147.GameCore.Level;

namespace Project147.Tests.EditMode.GameCore.Choices
{
    public sealed class RunChoiceResolverTests
    {
        [Test]
        public void Apply_WhenChoiceIsNull_Throws()
        {
            var resolver = new RunChoiceResolver();

            Assert.Throws<ArgumentNullException>(() => resolver.Apply(
                null,
                new BaseState(10),
                new CurrencyWallet(100)));
        }

        [Test]
        public void Apply_WhenBaseIsNull_Throws()
        {
            var resolver = new RunChoiceResolver();

            Assert.Throws<ArgumentNullException>(() => resolver.Apply(
                CreateScrapChoice(),
                null,
                new CurrencyWallet(100)));
        }

        [Test]
        public void Apply_WhenWalletIsNull_Throws()
        {
            var resolver = new RunChoiceResolver();

            Assert.Throws<ArgumentNullException>(() => resolver.Apply(
                CreateScrapChoice(),
                new BaseState(10),
                null));
        }

        [Test]
        public void Apply_WhenRunModifiersAreNull_Throws()
        {
            var resolver = new RunChoiceResolver();

            Assert.Throws<ArgumentNullException>(() => resolver.Apply(
                CreateScrapChoice(),
                new BaseState(10),
                new CurrencyWallet(100),
                null));
        }

        [Test]
        public void Apply_WhenChoiceAddsScrap_ReturnsWalletWithAddedScrap()
        {
            var resolver = new RunChoiceResolver();
            var baseState = new BaseState(10).ApplyLeakDamage(2);
            var wallet = new CurrencyWallet(100);

            var result = resolver.Apply(CreateScrapChoice(), baseState, wallet);

            Assert.That(result.Wallet.Balance, Is.EqualTo(145));
            Assert.That(result.BaseState, Is.SameAs(baseState));
            Assert.That(result.RunModifiers.HasNextTowerDiscount, Is.False);
            Assert.That(wallet.Balance, Is.EqualTo(100));
        }

        [Test]
        public void Apply_WhenChoiceRepairsBase_ReturnsBaseWithRepairedHealth()
        {
            var resolver = new RunChoiceResolver();
            var baseState = new BaseState(10).ApplyLeakDamage(3);
            var wallet = new CurrencyWallet(100);

            var result = resolver.Apply(
                new RunChoiceDefinition("field-repair", "Field Repair", RunChoiceEffectType.RepairBase, 2),
                baseState,
                wallet);

            Assert.That(result.BaseState.CurrentHealth, Is.EqualTo(9));
            Assert.That(result.Wallet, Is.SameAs(wallet));
            Assert.That(baseState.CurrentHealth, Is.EqualTo(7));
        }

        [Test]
        public void Apply_WhenChoiceAddsNextTowerDiscount_ReturnsModifiersWithDiscount()
        {
            var resolver = new RunChoiceResolver();
            var baseState = new BaseState(10);
            var wallet = new CurrencyWallet(100);
            var runModifiers = new RunModifierState().AddNextTowerDiscount(10);

            var result = resolver.Apply(
                new RunChoiceDefinition(
                    "construction-credit",
                    "Construction Credit",
                    RunChoiceEffectType.AddNextTowerDiscount,
                    30),
                baseState,
                wallet,
                runModifiers);

            Assert.That(result.BaseState, Is.SameAs(baseState));
            Assert.That(result.Wallet, Is.SameAs(wallet));
            Assert.That(result.RunModifiers.NextTowerDiscountAmount, Is.EqualTo(40));
            Assert.That(runModifiers.NextTowerDiscountAmount, Is.EqualTo(10));
        }

        [Test]
        public void Apply_WhenChoiceAddsNextWaveTowerDamagePercent_ReturnsModifiersWithPendingBoost()
        {
            var resolver = new RunChoiceResolver();
            var baseState = new BaseState(10);
            var wallet = new CurrencyWallet(100);
            var runModifiers = new RunModifierState().AddNextWaveTowerDamagePercent(10);

            var result = resolver.Apply(
                new RunChoiceDefinition(
                    "overclock",
                    "Overclock",
                    RunChoiceEffectType.AddNextWaveTowerDamagePercent,
                    25),
                baseState,
                wallet,
                runModifiers);

            Assert.That(result.BaseState, Is.SameAs(baseState));
            Assert.That(result.Wallet, Is.SameAs(wallet));
            Assert.That(result.RunModifiers.PendingWaveTowerDamagePercent, Is.EqualTo(35));
            Assert.That(runModifiers.PendingWaveTowerDamagePercent, Is.EqualTo(10));
        }

        [Test]
        public void Apply_WhenChoiceAddsNextWaveTowerFireRatePercent_ReturnsModifiersWithPendingBoost()
        {
            var resolver = new RunChoiceResolver();
            var baseState = new BaseState(10);
            var wallet = new CurrencyWallet(100);
            var runModifiers = new RunModifierState().AddNextWaveTowerFireRatePercent(5);

            var result = resolver.Apply(
                new RunChoiceDefinition(
                    "rapid-loader",
                    "Rapid Loader",
                    RunChoiceEffectType.AddNextWaveTowerFireRatePercent,
                    20),
                baseState,
                wallet,
                runModifiers);

            Assert.That(result.BaseState, Is.SameAs(baseState));
            Assert.That(result.Wallet, Is.SameAs(wallet));
            Assert.That(result.RunModifiers.PendingWaveTowerFireRatePercent, Is.EqualTo(25));
            Assert.That(runModifiers.PendingWaveTowerFireRatePercent, Is.EqualTo(5));
        }

        [Test]
        public void Apply_WhenChoiceEffectIsUnknown_Throws()
        {
            var resolver = new RunChoiceResolver();
            var unknownChoice = new RunChoiceDefinition(
                "unknown-effect",
                "Unknown Effect",
                (RunChoiceEffectType)999,
                1);

            Assert.Throws<ArgumentOutOfRangeException>(() => resolver.Apply(
                unknownChoice,
                new BaseState(10),
                new CurrencyWallet(100)));
        }

        private static RunChoiceDefinition CreateScrapChoice()
        {
            return new RunChoiceDefinition(
                "salvage-drop",
                "Salvage Drop",
                RunChoiceEffectType.AddScrap,
                45);
        }
    }
}
