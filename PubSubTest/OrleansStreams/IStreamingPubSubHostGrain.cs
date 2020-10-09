using Orleans;
using System.Threading.Tasks;

namespace PubSubTest.OrleansStreams
{
    public interface IStreamingPubSubHostGrain : IGrainWithStringKey
    {
        Task SendAsync(string payload);

        Task ActivateAsync();
    }
}
