using System;

namespace Project147.GameCore.Level
{
    public sealed class CampaignProgressApplicationResult
    {
        public CampaignProgressApplicationResult(
            CampaignProgressState campaign,
            LevelProgressApplicationResult levelResult)
        {
            Campaign = campaign ?? throw new ArgumentNullException(nameof(campaign));
            LevelResult = levelResult ?? throw new ArgumentNullException(nameof(levelResult));
        }

        public CampaignProgressState Campaign { get; }

        public LevelProgressApplicationResult LevelResult { get; }
    }
}
