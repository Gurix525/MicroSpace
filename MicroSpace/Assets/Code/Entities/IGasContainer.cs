using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public interface IGasContainer : IEntity
    {
        public Dictionary<int, int> Gasses { get; }
    }
}