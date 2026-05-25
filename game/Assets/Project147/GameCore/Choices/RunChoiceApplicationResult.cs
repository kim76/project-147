using System;
using Project147.GameCore.Level;

namespace Project147.GameCore.Choices
{
    public sealed class RunChoiceApplicationResult
    {
        public RunChoiceApplicationResult(BaseState baseState, CurrencyWallet wallet)
            : this(baseState, wallet, new RunModifierState())
        {
        }

        public RunChoiceApplicationResult(
            BaseState baseState,
            CurrencyWallet wallet,
            RunModifierState runModifiers)
        {
            BaseState = baseState ?? throw new ArgumentNullException(nameof(baseState));
            Wallet = wallet ?? throw new ArgumentNullException(nameof(wallet));
            RunModifiers = runModifiers ?? throw new ArgumentNullException(nameof(runModifiers));
        }

        public BaseState BaseState { get; }

        public CurrencyWallet Wallet { get; }

        public RunModifierState RunModifiers { get; }
    }
}
