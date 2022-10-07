using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Data
{
    [Serializable]
    public class FloorData : PartData
    {
        public bool IsExposed = false;

        public FloorData UpFloor = null;
        public FloorData DownFloor = null;
        public FloorData LeftFloor = null;
        public FloorData RightFloor = null;

        public RoomData Room = null;
    }
}