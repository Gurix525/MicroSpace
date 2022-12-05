using System.Collections.Generic;
using Miscellaneous;

namespace Entities
{
    public interface IGasContainer : IIdentifiable
    {
        public Dictionary<int, int> Gasses { get; }
    }
}