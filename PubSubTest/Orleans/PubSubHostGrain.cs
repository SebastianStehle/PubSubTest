using Orleans;
using PubSubTest.Placement;
using System;
using System.Threading.Tasks;

namespace PubSubTest.Orleans
{
    [LocalPlacement]
    public sealed class PubSubHostGrain : Grain, IPubSubHostGrain
    {
        private readonly OrleansPubSub pubSub;
        private IPubSubBrokerGrain broker;
        private string id;

        public PubSubHostGrain(OrleansPubSub pubSub)
        {
            this.pubSub = pubSub;
        }

        public override Task OnActivateAsync()
        {
            id = this.GetPrimaryKeyString();

            broker = GrainFactory.GetGrain<IPubSubBrokerGrain>(Constants.BrokerId);

            RegisterTimer(x => ReportIamAliveAsync(), null, TimeSpan.Zero, Constants.ReportAlivePeriod);

            DelayDeactivation(TimeSpan.FromDays(100000));

            return base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            return ReportIamDeadAsync();
        }

        private Task ReportIamAliveAsync()
        {
            return broker.IAmAliveAsync(id);
        }

        private Task ReportIamDeadAsync()
        {
            return broker.IamDeadAsync(id);
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
