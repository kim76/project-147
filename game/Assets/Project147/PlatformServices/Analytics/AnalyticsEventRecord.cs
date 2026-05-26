using System;
using System.Collections.Generic;

namespace Project147.PlatformServices.Analytics
{
    public sealed class AnalyticsEventRecord
    {
        public AnalyticsEventRecord(string name, IReadOnlyDictionary<string, string> properties)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Analytics event record name is required.", nameof(name));
            }

            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            foreach (var property in properties)
            {
                if (string.IsNullOrWhiteSpace(property.Key))
                {
                    throw new ArgumentException("Analytics event property names cannot be blank.", nameof(properties));
                }
            }

            Name = name;
            Properties = new Dictionary<string, string>(properties);
        }

        public string Name { get; }

        public IReadOnlyDictionary<string, string> Properties { get; }
    }
}
