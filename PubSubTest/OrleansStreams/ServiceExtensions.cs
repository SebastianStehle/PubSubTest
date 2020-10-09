using Microsoft.Extensions.DependencyInjection;
using Orleans.Hosting;
using PubSubTest.Placement;

namespace PubSubTest.OrleansStreams
{
    public static class ServiceExtensions
    {
        public static void AddPubSub(this ISiloBuilder siloBuilder)
        {
            siloBuilder.AddPlacementDirector<LocalPlacementStrategy, LocalPlacementDirector>();

            siloBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<OrleansStreamingPubSub>();
                services.AddSingleton<IPubSub>(c => c.GetRequiredService<OrleansStreamingPubSub>());
            });

            siloBuilder.AddSimpleMessageStreamProvider(Constants.StreamProviderName);
            siloBuilder.AddMemoryGrainStorage("PubSubStore");

            siloBuilder.AddStartupTask<OrleansStreamingPubSub>();
        }
    }
}
