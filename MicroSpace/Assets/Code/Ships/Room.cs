using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Ships
{
    [Serializable]
    public class Room
    {
        public int Id;

        public Room(int id)
        {
            Id = id;
        }
    }
}