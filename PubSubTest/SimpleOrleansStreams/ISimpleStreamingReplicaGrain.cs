using Orleans;
using Orleans.Runtime;
using PubSubTest.SimpleOrleansStreams;
using System;
using System.Threading.Tasks;

namespace PubSubTest.SimpleOrleansStreams
{
    public interface ISimpleStreamingReplicaGrain : IGrainWithStringKey
    {
        public ValueTask<string> GetAsync();
    }
}

namespace Orleans
{
    /// <summary>
    /// Discoverability extensions for <see cref="ISimpleStreamingReplicaGrain"/>.
    /// </summary>
    public static class SimpleStreamingReplicaGrainFactoryExtensions
    {
        /// <summary>
        /// Gets the local replica grain. Use of this grain requires a co-hosted deployment (caller api and silo in the same process so the local <see cref="SiloAddress"/> is available.
        /// </summary>
        /// <param name="factory">The factory to use.</param>
        /// <param name="address">This must be the local silo address.</param>
        /// <param name="key">Sharding key of the data graph.</param>
        public static ISimpleStreamingReplicaGrain GetSimpleStreamingReplicaGrain(this IGrainFactory factory, SiloAddress address, string key)
        {
            if (factory is null) throw new ArgumentNullException(nameof(key));

            key = key.Trim().ToUpperInvariant();
            var parsable = address.ToParsableString();

            return factory.GetGrain<ISimpleStreamingReplicaGrain>($"{key}|{parsable}");
        }
    }

    /// <summary>
    /// Key extensions for <see cref="ISimpleStreamingReplicaGrain"/>.
    /// </summary>
    public static class SimpleStreamingReplicaGrainKeyExtensions
    {
        public static (string key, SiloAddress address) GetCompositeKey(this ISimpleStreamingReplicaGrain grain)
        {
            if (grain is null) throw new ArgumentNullException(nameof(grain));

            var components = grain.GetPrimaryKeyString().Split('|');
            var key = components[0];
            var address = SiloAddress.FromParsableString(components[1]);

            return (key, address);
        }
    }
}