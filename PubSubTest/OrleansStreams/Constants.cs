using System;

namespace PubSubTest.OrleansStreams
{
    public static class Constants
    {
        public static readonly TimeSpan SendTimeout = TimeSpan.FromMilliseconds(100);

        public const string BrokerId = "DEFAULT";
        public const string StreamProviderName = "PubSub";

        public static readonly Guid StreamId = default;
    }
}
