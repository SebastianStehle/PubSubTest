using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using ProtoBuf;
using System;
using System.Threading.Tasks;

namespace PubSubTest.SimpleOrleansStreams
{
    public class SimpleStreamingPublisherGrain : Grain, ISimpleStreamingPublisherGrain
    {
        private readonly MySimpleStreamingOptions _options;
        private readonly IPersistentState<SimpleStreamingPublisherGrainState> _state;

        public SimpleStreamingPublisherGrain(IOptions<MySimpleStreamingOptions> options, [PersistentState("State")] IPersistentState<SimpleStreamingPublisherGrainState> state)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _state = state ?? throw new ArgumentNullException(nameof(state));
        }

        private IAsyncStream<string> _stream;

        public override Task OnActivateAsync()
        {
            // keep the stream ref handy
            var key = this.GetPrimaryKeyString();
            _stream = GetStreamProvider(_options.StreamProviderName).GetStream<string>(Guid.Empty, key);

            return base.OnActivateAsync();
        }

        public async Task SendAsync(string payload)
        {
            _state.State.Payload = payload;
            await _state.WriteStateAsync();

            for (var i = 0; i < _options.RetryCount; ++i)
            {
                try
                {
                    await _stream.OnNextAsync(payload);
                    break;
                }
                catch
                {
                    if (i >= _options.RetryCount - 1)
                    {
                        throw;
                    }
                }
            }
        }

        public ValueTask<string> GetAsync() => new ValueTask<string>(_state.State.Payload);
    }

    [ProtoContract]
    public class SimpleStreamingPublisherGrainState
    {
        [ProtoMember(1)]
        public string Payload { get; set; }
    }
}