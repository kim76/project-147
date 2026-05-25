using System;
using Project147.GameCore.Level;

namespace Project147.GameCore.Choices
{
    public sealed class RunChoiceResolver
    {
        public RunChoiceApplicationResult Apply(
            RunChoiceDefinition choice,
            BaseState baseState,
            CurrencyWallet wallet)
        {
            return Apply(choice, baseState, wallet, new RunModifierState());
        }

        public RunChoiceApplicationResult Apply(
            RunChoiceDefinition choice,
            BaseState baseState,
            CurrencyWallet wallet,
            RunModifierState runModifiers)
        {
            if (choice == null)
            {
                throw new ArgumentNullException(nameof(choice));
            }

            if (baseState == null)
            {
                throw new ArgumentNullException(nameof(baseState));
            }

            if (wallet == null)
            {
                throw new ArgumentNullException(nameof(wallet));
            }

            if (runModifiers == null)
            {
                throw new ArgumentNullException(nameof(runModifiers));
            }

            switch (choice.EffectType)
            {
                case RunChoiceEffectType.AddScrap:
                    return new RunChoiceApplicationResult(baseState, wallet.Add(choice.Amount), runModifiers);
                case RunChoiceEffectType.RepairBase:
                    return new RunChoiceApplicationResult(baseState.Repair(choice.Amount), wallet, runModifiers);
                case RunChoiceEffectType.AddNextTowerDiscount:
                    return new RunChoiceApplicationResult(
                        baseState,
                        wallet,
                        runModifiers.AddNextTowerDiscount(choice.Amount));
                default:
                    throw new ArgumentOutOfRangeException(nameof(choice), "Unknown run choice effect type.");
            }
        }
    }
}
