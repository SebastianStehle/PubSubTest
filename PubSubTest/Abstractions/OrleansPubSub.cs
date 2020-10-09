using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace PubSubTest.Abstractions
{
    public sealed class OrleansPubSub : IPubSub, IStartupTask
    {
        private readonly IPubSubHostGrain hostGrain;
        private readonly IPubSubBrokerGrain brokerGrain;
        private readonly ConcurrentBag<Action<string>> subscribers = new ConcurrentBag<Action<string>>();
        private readonly ILogger<OrleansPubSub> logger;

        public OrleansPubSub(IGrainFactory grainFactory, ILogger<OrleansPubSub> logger)
        {
            brokerGrain = grainFactory.GetGrain<IPubSubBrokerGrain>(Constants.BrokerId);

            hostGrain = grainFactory.GetGrain<IPubSubHostGrain>(Guid.NewGuid());

            this.logger = logger;
        }

        public Task Execute(CancellationToken cancellationToken)
        {
            return hostGrain.ActivateAsync();
        }

        public Task PublishAsync(string payload)
        {
            return brokerGrain.SendAsync(payload);
        }

        public Task<int> GetClusterSizeAsync()
        {
            return brokerGrain.GetClusterSizeAsync();
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
