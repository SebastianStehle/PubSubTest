using Orleans.Placement;
using Orleans.Runtime;
using System;

namespace PubSubTest.Placement
{
    [Serializable]
    public class LocalPlacementStrategy : PlacementStrategy
    {
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class LocalPlacementAttribute : PlacementAttribute
    {
        public LocalPlacementAttribute()
            : base(new LocalPlacementStrategy())
        {
        }
    }
}
