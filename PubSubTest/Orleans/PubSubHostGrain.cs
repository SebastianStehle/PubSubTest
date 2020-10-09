using Orleans;
using System;
using System.Threading.Tasks;

namespace PubSubTest.Orleans
{
    [LocalPlacement]
    public sealed class PubSubHostGrain : Grain, IPubSubHostGrain
    {
        private readonly OrleansPubSub pubSub;
        private IPubSubBrokerGrain broker;

        public PubSubHostGrain(OrleansPubSub pubSub)
        {
            this.pubSub = pubSub;
        }

        public override Task OnActivateAsync()
        {
            broker = GrainFactory.GetGrain<IPubSubBrokerGrain>(Constants.BrokerId);

            RegisterTimer(x => ReportIamAliveAsync(), null, TimeSpan.Zero, Constants.ReportAlivePeriod);

            return base.OnActivateAsync();
        }

        private Task ReportIamAliveAsync()
        {
            var id = this.GetPrimaryKey();

            return broker.IAmAliveAsync(id);
        }

        public Task ActivateAsync()
        {
            return ReportIamAliveAsync();
        }

        public Task SendAsync(string payload)
        {
            pubSub.Publish(payload);

            return Task.CompletedTask;
        }
    }
}
