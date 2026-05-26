using Project147.GameCore.Level;

namespace Project147.PlatformServices.Save
{
    public interface ICampaignProgressStore
    {
        bool Exists { get; }

        CampaignProgressState LoadOrDefault();

        void Save(CampaignProgressState campaign);

        void Clear();
    }
}
