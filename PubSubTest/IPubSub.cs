using System;
using System.Threading.Tasks;

namespace PubSubTest
{
    public interface IPubSub
    {
        Task PublishAsync(string payload);

        Task<int> GetClusterSizeAsync();

        void Subscribe(Action<string> subscriber);
    }
}
