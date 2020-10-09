using Orleans;
using System.Threading.Tasks;

namespace PubSubTest.Abstractions
{
    public interface IPubSubHostGrain : IGrainWithGuidKey
    {
        Task ActivateAsync();

        Task SendAsync(string payload);
    }
}
