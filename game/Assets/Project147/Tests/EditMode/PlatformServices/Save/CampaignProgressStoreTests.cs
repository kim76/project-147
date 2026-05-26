using NUnit.Framework;
using Project147.GameCore.Level;
using Project147.PlatformServices.Save;

namespace Project147.Tests.EditMode.PlatformServices.Save
{
    public sealed class CampaignProgressStoreTests
    {
        [Test]
        public void LoadOrDefault_WhenStorageIsEmpty_ReturnsEmptyCampaign()
        {
            var store = CreateStore();

            var result = store.LoadOrDefault();

            Assert.That(result.TotalStars, Is.EqualTo(0));
        }

        [Test]
        public void Save_StoresCampaignForLaterLoad()
        {
            var storage = new InMemoryTextSaveStorage();
            var store = CreateStore(storage);
            var campaign = new CampaignProgressState()
                .ApplyRunSummary("debug-relay-yard", CreateVictory())
                .Campaign;

            store.Save(campaign);

            Assert.That(store.Exists, Is.True);
            Assert.That(store.LoadOrDefault().TotalStars, Is.EqualTo(3));
        }

        [Test]
        public void Clear_RemovesStoredCampaign()
        {
            var storage = new InMemoryTextSaveStorage();
            var store = CreateStore(storage);
            store.Save(new CampaignProgressState()
                .ApplyRunSummary("debug-relay-yard", CreateVictory())
                .Campaign);

            store.Clear();

            Assert.That(store.Exists, Is.False);
            Assert.That(store.LoadOrDefault().TotalStars, Is.EqualTo(0));
        }

        private static CampaignProgressStore CreateStore()
        {
            return CreateStore(new InMemoryTextSaveStorage());
        }

        private static CampaignProgressStore CreateStore(ITextSaveStorage storage)
        {
            return new CampaignProgressStore(storage, new CampaignProgressSaveCodec());
        }

        private static RunSummaryState CreateVictory()
        {
            return new RunSummaryState()
                .RecordWaveCleared(25, true)
                .RecordWaveCleared(25, true)
                .RecordWaveCleared(25, true)
                .Complete(RunOutcome.Victory);
        }
    }
}
