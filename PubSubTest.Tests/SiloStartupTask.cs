using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PubSubTest.Tests
{
    public sealed class SiloStartupTask : IStartupTask, IDisposable
    {
        private static readonly List<SiloStartupTask> allSilos = new List<SiloStartupTask>();
        private readonly HashSet<string> received = new HashSet<string>();

        public IPubSub PubSub { get; }

        public static IReadOnlyCollection<SiloStartupTask> All => allSilos;

        public IReadOnlyCollection<string> Received => received;

        public SiloStartupTask(IPubSub pubSub)
        {
            PubSub = pubSub;
        }

        public static void Clear()
        {
            allSilos.Clear();
        }

        public Task Execute(CancellationToken cancellationToken)
        {
            lock (allSilos)
            {
                allSilos.Add(this);
            }

            PubSub.Subscribe(message =>
            {
                received.Add(message);
            });

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            lock (allSilos)
            {
                allSilos.Remove(this);
            }
        }
    }
}
