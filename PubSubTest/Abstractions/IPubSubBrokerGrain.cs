using Orleans;
using System;
using System.Threading.Tasks;

namespace PubSubTest.Abstractions
{
    public interface IPubSubBrokerGrain : IGrainWithStringKey
    {
        Task IAmAliveAsync(Guid hostId);

        Task IamDeadAsync(Guid hostId);

        Task SendAsync(string payload);

        Task<int> GetClusterSizeAsync();
    }
}
