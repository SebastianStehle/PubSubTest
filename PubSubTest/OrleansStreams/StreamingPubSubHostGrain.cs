using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using PubSubTest.Placement;
using System;
using System.Threading.Tasks;

namespace PubSubTest.OrleansStreams
{
    [LocalPlacement]
    public sealed class StreamingPubSubHostGrain : Grain, IStreamingPubSubHostGrain
    {
        private readonly OrleansStreamingPubSub pubSub;
        private readonly ILogger<OrleansStreamingPubSub> logger;
        private StreamSubscriptionHandle<string> subscription;
        private IAsyncStream<string> stream;

        public StreamingPubSubHostGrain(OrleansStreamingPubSub pubSub, ILogger<OrleansStreamingPubSub> logger)
        {
            this.pubSub = pubSub;
            this.logger = logger;
        }

        public override async Task OnActivateAsync()
        {
            var streamProvider = GetStreamProvider(Constants.StreamProviderName);

            stream = streamProvider.GetStream<string>(Constants.StreamId, Constants.StreamProviderName);

            subscription = await stream.SubscribeAsync((data, token) =>
            {
                pubSub.Publish(data);

                return Task.CompletedTask;
            });

            DelayDeactivation(TimeSpan.FromDays(100000));
        }

        public override Task OnDeactivateAsync()
        {
            return subscription.UnsubscribeAsync();
        }

        public Task ActivateAsync()
        {
            return Task.CompletedTask;
        }

        public async Task SendAsync(string payload)
        {
            try
            {
                var timeout = Task.Delay(Constants.SendTimeout);

                var completed = await Task.WhenAny(timeout, stream.OnNextAsync(payload));

                if (completed == timeout)
                {
                    logger.LogWarning("Failed to send message within {time}", Constants.SendTimeout);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to publish message");
            }
        }
    }
}
