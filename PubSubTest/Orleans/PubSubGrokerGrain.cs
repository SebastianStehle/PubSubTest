using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PubSubTest.Orleans
{
    public sealed class PubSubGrokerGrain : Grain, IPubSubBrokerGrain
    {
        private readonly Dictionary<Guid, DateTime> hosts = new Dictionary<Guid, DateTime>();
        private readonly ILogger<OrleansPubSub> logger;

        public PubSubGrokerGrain(ILogger<OrleansPubSub> logger)
        {
            this.logger = logger;
        }

        public override Task OnActivateAsync()
        {
            RegisterTimer(x =>
            {
                Cleanup();

                return Task.CompletedTask;
            }, null, Constants.DeadTimeout, Constants.DeadTimeout);

            return Task.CompletedTask;
        }

        public Task<int> GetClusterSizeAsync()
        {
            Cleanup();

            return Task.FromResult(hosts.Count);
        }

        private void Cleanup()
        {
            if (hosts.Count > 0)
            {
                var now = DateTime.UtcNow;

                var dead = new Guid[hosts.Count];

                var i = 0;

                foreach (var (hostId, lastSeen) in hosts)
                {
                    var timeSinceLastSeen = now - lastSeen;

                    if (timeSinceLastSeen > Constants.DeadTimeout)
                    {
                        dead[i++] = hostId;
                    }
                }

                foreach (var hostId in dead)
                {
                    if (hostId != default)
                    {
                        hosts.Remove(hostId);
                    }
                }
            }
        }

        public Task IAmAliveAsync(Guid hostId)
        {
            hosts[hostId] = DateTime.UtcNow;

            return Task.CompletedTask;
        }

        public Task IamDeadAsync(Guid hostId)
        {
            hosts.Remove(hostId);

            return Task.CompletedTask;
        }

        public Task SendAsync(string payload)
        {
            if (hosts.Count == 0)
            {
                return Task.CompletedTask;
            }

            var tasks = new Task[hosts.Count];

            var i = 0;

            foreach (var hostId in hosts.Keys)
            {
                tasks[i++] = SendAsync(hostId, payload);
            }

            return Task.WhenAll(tasks);
        }

        private async Task SendAsync(Guid hostId, string payload)
        {
            var host = GrainFactory.GetGrain<IPubSubHostGrain>(hostId);

            try
            {
                var timeout = Task.Delay(Constants.SendTimeout);

                var completed = await Task.WhenAny(timeout, host.SendAsync(payload));
                
                if (completed == timeout)
                {
                    logger.LogWarning("Failed to send message to {host} within {timespan}", hostId, Constants.SendTimeout);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to publish message to {host}", hostId);
            }
        }
    }
}
