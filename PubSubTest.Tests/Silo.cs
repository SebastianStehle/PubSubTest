using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PubSubTest.Tests
{
    public sealed class Silo : IStartupTask, IDisposable
    {
        private static readonly List<Silo> allSilos = new List<Silo>();
        private readonly HashSet<string> received = new HashSet<string>();

        public IPubSub PubSub { get; }

        public static IReadOnlyCollection<Silo> All => allSilos;

        public IReadOnlyCollection<string> Received => received;

        public Silo(IPubSub pubSub)
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
