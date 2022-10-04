using Assets.Code.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Data.PartDataImplementations
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

        public static implicit operator FloorData(Floor floor)
        {
            var floorData = floor.FloorData ?? new();
            //wallData.Name = wall.Name;
            //wallData.Resilience = wall.Resilience;
            //wallData.MaxEndurance = wall.MaxEndurance;
            //wallData.CurrentEndurance = wall.CurrentEndurance;
            floorData.LocalPosition = new int[]
            {
                (int) Math.Round(floor.gameObject.transform.localPosition.x),
                (int) Math.Round(floor.gameObject.transform.localPosition.y)
            };
            floorData.LocalRotation = floor.gameObject.transform.localEulerAngles.z;
            floor.FloorData = floorData;
            return floorData;
        }
    }
}