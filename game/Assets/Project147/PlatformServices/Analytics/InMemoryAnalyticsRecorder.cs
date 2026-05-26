using System;
using System.Collections.Generic;
using System.Linq;

namespace Project147.PlatformServices.Analytics
{
    public sealed class InMemoryAnalyticsRecorder
    {
        private readonly IReadOnlyList<AnalyticsEventDefinition> definitions;
        private readonly List<AnalyticsEventRecord> records = new List<AnalyticsEventRecord>();

        public InMemoryAnalyticsRecorder(IReadOnlyList<AnalyticsEventDefinition> definitions)
        {
            if (definitions == null)
            {
                throw new ArgumentNullException(nameof(definitions));
            }

            if (definitions.Any(definition => definition == null))
            {
                throw new ArgumentException("Analytics definitions cannot contain null entries.", nameof(definitions));
            }

            if (definitions.Select(definition => definition.Name).Distinct(StringComparer.Ordinal).Count() != definitions.Count)
            {
                throw new ArgumentException("Analytics definitions must have unique names.", nameof(definitions));
            }

            this.definitions = new List<AnalyticsEventDefinition>(definitions);
        }

        public IReadOnlyList<AnalyticsEventRecord> Records
        {
            get { return records; }
        }

        public void Track(string eventName, IReadOnlyDictionary<string, string> properties)
        {
            var definition = definitions.SingleOrDefault(candidate => candidate.Name == eventName);

            if (definition == null)
            {
                throw new ArgumentException($"Unknown analytics event '{eventName}'.", nameof(eventName));
            }

            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            foreach (var requiredProperty in definition.RequiredProperties)
            {
                if (!properties.ContainsKey(requiredProperty))
                {
                    throw new ArgumentException(
                        $"Analytics event '{eventName}' is missing required property '{requiredProperty}'.",
                        nameof(properties));
                }
            }

            records.Add(new AnalyticsEventRecord(eventName, properties));
        }
    }
}
