using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Placement;
using Orleans.Runtime;
using Orleans.Streams;
using System;
using System.Threading.Tasks;

namespace PubSubTest.SimpleOrleansStreams
{
    [PreferLocalPlacement]
    public class SimpleStreamingReplicaGrain : Grain, ISimpleStreamingReplicaGrain, IAsyncObserver<string>
    {
        private readonly Silo _silo;
        private readonly MySimpleStreamingOptions _options;

        public SimpleStreamingReplicaGrain(Silo silo, IOptions<MySimpleStreamingOptions> options)
        {
            _silo = silo ?? throw new ArgumentNullException(nameof(silo));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        private string _payload;
        private StreamSubscriptionHandle<string> _handle;

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();

            var (key, address) = this.GetCompositeKey();
            var stream = GetStreamProvider(_options.StreamProviderName).GetStream<string>(Guid.Empty, key);

            // the most common activation will happen in the local silo of the query call
            _handle = await stream.ResumeOrSubscribeAsync(this);

            // however if this activation is induced by the streaming provider (e.g. after a silo crash) then it will activate in the wrong silo
            // in that case we want to gracefully discard this replica
            if (address != _silo.SiloAddress)
            {
                DeactivateOnIdle();
            }
        }

        public override async Task OnDeactivateAsync()
        {
            await _handle.UnsubscribeAsync();

            await base.OnDeactivateAsync();
        }

        public ValueTask<string> GetAsync() => new ValueTask<string>(_payload);

        public Task OnNextAsync(string item, StreamSequenceToken token = null)
        {
            _payload = item;

            return Task.CompletedTask;
        }

        public Task OnCompletedAsync() => Task.CompletedTask;

        public Task OnErrorAsync(Exception ex) => Task.CompletedTask;
    }
}