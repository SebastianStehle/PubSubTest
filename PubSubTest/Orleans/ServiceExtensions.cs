using Microsoft.Extensions.DependencyInjection;
using Orleans.Hosting;

namespace PubSubTest.Orleans
{
    public static class ServiceExtensions
    {
        public static void AddPubSub(this ISiloBuilder siloBuilder)
        {
            siloBuilder.AddPlacementDirector<LocalPlacementStrategy, LocalPlacementDirector>();

            siloBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<OrleansPubSub>();
                services.AddSingleton<IPubSub>(c => c.GetRequiredService<OrleansPubSub>());
            });

            siloBuilder.AddStartupTask<OrleansPubSub>();
        }
    }
}
