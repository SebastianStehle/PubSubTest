using Orleans;
using System.Threading.Tasks;

namespace PubSubTest.Orleans
{
    public interface IPubSubBrokerGrain : IGrainWithStringKey
    {
        Task IAmAliveAsync(string hostId);

        Task IamDeadAsync(string hostId);

        Task SendAsync(string payload);

        Task<int> GetClusterSizeAsync();
    }
}
