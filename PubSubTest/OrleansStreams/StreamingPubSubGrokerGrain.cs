using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using System;
using System.Threading.Tasks;

namespace PubSubTest.OrleansStreams
{
    public sealed class StreamingPubSubGrokerGrain : Grain, IStreamingPubSubBrokerGrain
    {
        private readonly ILogger<OrleansStreamingPubSub> logger;
        private IAsyncStream<string> stream;

        public StreamingPubSubGrokerGrain(ILogger<OrleansStreamingPubSub> logger)
        {
            this.logger = logger;
        }

        public override Task OnActivateAsync()
        {
            var streamProvider = GetStreamProvider(Constants.StreamProviderName);

            stream = streamProvider.GetStream<string>(Constants.StreamId, Constants.StreamProviderName);

            return base.OnActivateAsync();
        }

        public Task<int> GetClusterSizeAsync()
        {
            return Task.FromResult(-1);
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
