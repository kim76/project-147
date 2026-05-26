using System;
using Project147.GameCore.Level;

namespace Project147.PlatformServices.Save
{
    public sealed class CampaignProgressStore : ICampaignProgressStore
    {
        private readonly ITextSaveStorage storage;
        private readonly CampaignProgressSaveCodec codec;

        public CampaignProgressStore(ITextSaveStorage storage, CampaignProgressSaveCodec codec)
        {
            this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
            this.codec = codec ?? throw new ArgumentNullException(nameof(codec));
        }

        public bool Exists
        {
            get { return storage.Exists; }
        }

        public CampaignProgressState LoadOrDefault()
        {
            return storage.Exists
                ? codec.Decode(storage.ReadAllText())
                : new CampaignProgressState();
        }

        public void Save(CampaignProgressState campaign)
        {
            storage.WriteAllText(codec.Encode(campaign));
        }

        public void Clear()
        {
            storage.Delete();
        }
    }
}
