using System;
using System.Collections.Generic;
using System.Linq;

namespace Project147.PlatformServices.Analytics
{
    public sealed class AnalyticsEventDefinition
    {
        public AnalyticsEventDefinition(string name, IEnumerable<string> requiredProperties)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Analytics event name is required.", nameof(name));
            }

            if (requiredProperties == null)
            {
                throw new ArgumentNullException(nameof(requiredProperties));
            }

            var properties = requiredProperties.ToList();

            if (properties.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException("Analytics property names cannot be blank.", nameof(requiredProperties));
            }

            if (properties.Distinct(StringComparer.Ordinal).Count() != properties.Count)
            {
                throw new ArgumentException("Analytics property names must be unique.", nameof(requiredProperties));
            }

            Name = name;
            RequiredProperties = properties.AsReadOnly();
        }

        public string Name { get; }

        public IReadOnlyList<string> RequiredProperties { get; }
    }
}
