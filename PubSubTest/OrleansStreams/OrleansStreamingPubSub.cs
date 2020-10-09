using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace PubSubTest.OrleansStreams
{
    public sealed class OrleansStreamingPubSub : IPubSub, IStartupTask
    {
        private readonly IStreamingPubSubHostGrain hostGrain;
        private readonly ConcurrentBag<Action<string>> subscribers = new ConcurrentBag<Action<string>>();
        private readonly ILogger<OrleansStreamingPubSub> logger;

        public OrleansStreamingPubSub(IGrainFactory grainFactory, ILocalSiloDetails localSiloDetails, ILogger<OrleansStreamingPubSub> logger)
        {
            hostGrain = grainFactory.GetGrain<IStreamingPubSubHostGrain>(localSiloDetails.SiloAddress.ToParsableString());

            this.logger = logger;
        }

        public Task Execute(CancellationToken cancellationToken)
        {
            return hostGrain.ActivateAsync();
        }

        public Task PublishAsync(string payload)
        {
            return hostGrain.SendAsync(payload);
        }

        public Task<int> GetClusterSizeAsync()
        {
            return Task.FromResult(-1);
        }

        public void Publish(string payload)
        {
            foreach (var subscriber in subscribers)
            {
                try
                {
                    subscriber(payload);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Subscriber failed to handle message {payload}", payload);
                }
            }
        }

        public void Subscribe(Action<string> subscriber)
        {
            subscribers.Add(subscriber);
        }
    }
}
