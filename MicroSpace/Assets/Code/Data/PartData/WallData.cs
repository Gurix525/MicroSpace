using Assets.Code.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Data.PartDataImplementations
{
    [Serializable]
    public class WallData : PartData
    {
        public bool IsExposed = false;

        public WallData UpWall = null;
        public WallData DownWall = null;
        public WallData LeftWall = null;
        public WallData RightWall = null;

        public RoomData Room = null;

        public static implicit operator WallData(Wall wall)
        {
            var wallData = wall.WallData ?? new();
            //wallData.Name = wall.Name;
            //wallData.Resilience = wall.Resilience;
            //wallData.MaxEndurance = wall.MaxEndurance;
            //wallData.CurrentEndurance = wall.CurrentEndurance;
            wallData.LocalPosition = new int[]
            {
                (int) Math.Round(wall.gameObject.transform.localPosition.x),
                (int) Math.Round(wall.gameObject.transform.localPosition.y)
            };
            wallData.LocalRotation = wall.gameObject.transform.localEulerAngles.z;
            wall.WallData = wallData;
            return wallData;
        }
    }
}