using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Ships
{
    [Serializable]
    public class SerializableWall : SerializableBlock
    {
        public SerializableWall(Block block) : base(block)
        {
        }
    }
}