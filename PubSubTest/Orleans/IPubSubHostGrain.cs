using Orleans;
using System.Threading.Tasks;

namespace PubSubTest.Orleans
{
    public interface IPubSubHostGrain : IGrainWithGuidKey
    {
        Task ActivateAsync();

        Task SendAsync(string payload);
    }
}
