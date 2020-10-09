using Orleans;
using System.Threading.Tasks;

namespace PubSubTest.Orleans
{
    public interface IPubSubHostGrain : IGrainWithStringKey
    {
        Task ActivateAsync();

        Task SendAsync(string payload);
    }
}
