using System;
using System.Threading.Tasks;

namespace Orleans.Streams
{
    public static class OrleansStreamExtensions
    {
        public static async Task<StreamSubscriptionHandle<T>> ResumeOrSubscribeAsync<T>(this IAsyncStream<T> stream, IAsyncObserver<T> observer)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));

            var handles = await stream.GetAllSubscriptionHandles();

            // resume or subscribe to the stream
            if (handles.Count > 0)
            {
                // resume the first subscription
                var handle = await handles[0].ResumeAsync(observer);

                // cleanup any accidental duplicates
                for (var i = 1; i < handles.Count; ++i)
                {
                    await handles[i].UnsubscribeAsync();
                }

                return handle;
            }
            else
            {
                return await stream.SubscribeAsync(observer);
            }
        }

        public static async Task UnsubscribeAllAsync<T>(this IAsyncStream<T> stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));

            var handles = await stream.GetAllSubscriptionHandles();

            foreach (var handle in handles)
            {
                await handle.UnsubscribeAsync();
            }
        }
    }
}