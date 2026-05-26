using System;
using NUnit.Framework;
using Project147.GameCore.Level;
using Project147.PlatformServices.Save;

namespace Project147.Tests.EditMode.PlatformServices.Save
{
    public sealed class CampaignProgressSaveCodecTests
    {
        [Test]
        public void EncodeAndDecode_RoundTripsCampaignProgress()
        {
            var codec = new CampaignProgressSaveCodec();
            var campaign = new CampaignProgressState()
                .ApplyRunSummary("debug-relay-yard", CreateVictory(stars: 3))
                .Campaign
                .ApplyRunSummary("debug-switchback", CreateVictory(stars: 2))
                .Campaign;

            var text = codec.Encode(campaign);
            var result = codec.Decode(text);

            Assert.That(result.TotalStars, Is.EqualTo(5));
            Assert.That(result.GetProgress("debug-relay-yard").BestStars, Is.EqualTo(3));
            Assert.That(result.GetProgress("debug-switchback").BestStars, Is.EqualTo(2));
        }

        [Test]
        public void Decode_WhenTextIsEmpty_ReturnsEmptyCampaign()
        {
            var codec = new CampaignProgressSaveCodec();

            var result = codec.Decode("");

            Assert.That(result.TotalStars, Is.EqualTo(0));
        }

        [Test]
        public void Decode_WhenHeaderIsUnknown_Throws()
        {
            var codec = new CampaignProgressSaveCodec();

            Assert.Throws<FormatException>(() => codec.Decode("wrong-version\n"));
        }

        [Test]
        public void Decode_WhenLineFieldCountIsInvalid_Throws()
        {
            var codec = new CampaignProgressSaveCodec();

            Assert.Throws<FormatException>(() => codec.Decode("project147-campaign-progress-v1\nonly-one-field\n"));
        }

        [Test]
        public void Encode_WhenCampaignIsNull_Throws()
        {
            var codec = new CampaignProgressSaveCodec();

            Assert.Throws<ArgumentNullException>(() => codec.Encode(null));
        }

        private static RunSummaryState CreateVictory(int stars)
        {
            var summary = new RunSummaryState()
                .RecordWaveCleared(25, true)
                .RecordWaveCleared(25, true)
                .RecordWaveCleared(25, true);

            for (var leakIndex = 0; leakIndex < LeaksForStars(stars); leakIndex++)
            {
                summary = summary.RecordAlienLeaked();
            }

            return summary.Complete(RunOutcome.Victory);
        }

        private static int LeaksForStars(int stars)
        {
            switch (stars)
            {
                case 3:
                    return 0;
                case 2:
                    return 1;
                case 1:
                    return 3;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stars), "Stars must be between one and three.");
            }
        }
    }
}
