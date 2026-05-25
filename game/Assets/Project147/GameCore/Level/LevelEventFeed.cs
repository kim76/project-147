using System;
using System.Collections.Generic;

namespace Project147.GameCore.Level
{
    public sealed class LevelEventFeed
    {
        public LevelEventFeed(int capacity)
            : this(capacity, new List<string>())
        {
        }

        private LevelEventFeed(int capacity, IReadOnlyList<string> entries)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Event feed capacity must be greater than zero.");
            }

            if (entries == null)
            {
                throw new ArgumentNullException(nameof(entries));
            }

            if (entries.Count > capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(entries), "Event feed entries cannot exceed capacity.");
            }

            Capacity = capacity;
            Entries = new List<string>(entries);
        }

        public int Capacity { get; }

        public IReadOnlyList<string> Entries { get; }

        public LevelEventFeed Add(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Event feed message is required.", nameof(message));
            }

            var entries = new List<string>(Entries)
            {
                message
            };

            while (entries.Count > Capacity)
            {
                entries.RemoveAt(0);
            }

            return new LevelEventFeed(Capacity, entries);
        }

        public LevelEventFeed Clear()
        {
            return new LevelEventFeed(Capacity);
        }
    }
}
