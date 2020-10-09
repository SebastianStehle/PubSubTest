using System;

namespace PubSubTest.Orleans
{
    public sealed class Constants
    {
        public static readonly TimeSpan DeadTimeout = TimeSpan.FromSeconds(10);
        public static readonly TimeSpan ReportAlivePeriod = DeadTimeout * 0.2;
        public static readonly TimeSpan SendTimeout = TimeSpan.FromMilliseconds(100);

        public const string BrokerId = "DEFAULT";
    }
}
