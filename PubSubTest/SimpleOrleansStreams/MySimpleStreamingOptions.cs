namespace PubSubTest.SimpleOrleansStreams
{
    public class MySimpleStreamingOptions
    {
        public string StreamProviderName { get; set; } = "SMS2";
        public int RetryCount { get; set; } = 3;
    }
}