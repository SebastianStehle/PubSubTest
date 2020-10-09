using Orleans;
using System.Threading.Tasks;

namespace PubSubTest.OrleansStreams
{
    public interface IStreamingPubSubBrokerGrain : IGrainWithStringKey
    {
        Task SendAsync(string payload);

        Task<int> GetClusterSizeAsync();
    }
}
