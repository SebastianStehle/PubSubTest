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
        private StreamSubscriptionHandle<string> subscription;

        public StreamingPubSubHostGrain(OrleansStreamingPubSub pubSub)
        {
            this.pubSub = pubSub;
        }

        public override async Task OnActivateAsync()
        {
            var streamProvider = GetStreamProvider(Constants.StreamProviderName);

            var stream = streamProvider.GetStream<string>(Constants.StreamId, Constants.StreamProviderName);

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
    }
}
