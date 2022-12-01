using System.Collections.Generic;

namespace Entities
{
    public interface IGasContainer : IEntity
    {
        public Dictionary<int, int> Gasses { get; }
    }
}