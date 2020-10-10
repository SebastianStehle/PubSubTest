using Orleans;
using PubSubTest.SimpleOrleansStreams;
using System;
using System.Threading.Tasks;

namespace PubSubTest.SimpleOrleansStreams
{
    public interface ISimpleStreamingPublisherGrain : IGrainWithStringKey
    {
        public ValueTask<string> GetAsync();

        public Task SendAsync(string payload);
    }
}

namespace Orleans
{
    /// <summary>
    /// Discoverability extensions for <see cref="ISimpleStreamingPublisherGrain"/>.
    /// </summary>
    public static class SimpleStreamingPublisherGrainFactoryExtensions
    {
        public static ISimpleStreamingPublisherGrain GetStreamingPublisherGrain(this IGrainFactory factory, string key)
        {
            if (factory is null) throw new ArgumentNullException(nameof(key));

            key = key.Trim().ToUpperInvariant();

            return factory.GetGrain<ISimpleStreamingPublisherGrain>(key);
        }
    }
}