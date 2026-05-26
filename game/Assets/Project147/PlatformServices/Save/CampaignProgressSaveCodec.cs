using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Project147.GameCore.Level;

namespace Project147.PlatformServices.Save
{
    public sealed class CampaignProgressSaveCodec
    {
        private const string Header = "project147-campaign-progress-v1";
        private const char Separator = '\t';

        public string Encode(CampaignProgressState campaign)
        {
            if (campaign == null)
            {
                throw new ArgumentNullException(nameof(campaign));
            }

            var builder = new StringBuilder();
            builder.AppendLine(Header);

            foreach (var level in CampaignProgressSnapshot.Capture(campaign).Levels)
            {
                builder.Append(Escape(level.LevelId));
                builder.Append(Separator);
                builder.Append(level.RunsCompleted.ToString(CultureInfo.InvariantCulture));
                builder.Append(Separator);
                builder.Append(level.Victories.ToString(CultureInfo.InvariantCulture));
                builder.Append(Separator);
                builder.Append(level.BestStars.ToString(CultureInfo.InvariantCulture));
                builder.Append(Separator);
                builder.Append(level.BestWavesCleared.ToString(CultureInfo.InvariantCulture));
                builder.Append(Separator);
                builder.AppendLine(level.BestPerfectWaves.ToString(CultureInfo.InvariantCulture));
            }

            return builder.ToString();
        }

        public CampaignProgressState Decode(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new CampaignProgressState();
            }

            var lines = text.Replace("\r\n", "\n").Split('\n');

            if (lines.Length == 0 || lines[0] != Header)
            {
                throw new FormatException("Campaign progress save has an unsupported format.");
            }

            var levels = new List<LevelProgressSnapshot>();

            for (var index = 1; index < lines.Length; index++)
            {
                var line = lines[index];

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var parts = line.Split(Separator);

                if (parts.Length != 6)
                {
                    throw new FormatException("Campaign progress save line has an invalid field count.");
                }

                levels.Add(new LevelProgressSnapshot(
                    Unescape(parts[0]),
                    ParseInt(parts[1]),
                    ParseInt(parts[2]),
                    ParseInt(parts[3]),
                    ParseInt(parts[4]),
                    ParseInt(parts[5])));
            }

            return new CampaignProgressSnapshot(levels).Restore();
        }

        private static int ParseInt(string value)
        {
            return int.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
        }

        private static string Escape(string value)
        {
            return Uri.EscapeDataString(value);
        }

        private static string Unescape(string value)
        {
            return Uri.UnescapeDataString(value);
        }
    }
}
