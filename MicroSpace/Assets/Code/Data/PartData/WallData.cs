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
        //public float CurrentEndurance;
        //public float[] LocalPosition = new float[2];
        //public float LocalRotation;
        //public float MaxEndurance;
        //public string Name;
        //public float Resilience;
        public bool IsExposed = false;

        public WallData UpWall = null;
        public WallData DownWall = null;
        public WallData LeftWall = null;
        public WallData RightWall = null;

        public RoomData Room = null;

        public static implicit operator WallData(Wall wall)
        {
            WallData wallData = new();
            wallData.Name = wall.Name;
            wallData.Resilience = wall.Resilience;
            wallData.MaxEndurance = wall.MaxEndurance;
            wallData.CurrentEndurance = wall.CurrentEndurance;
            wallData.LocalPosition = new int[]
            {
                (int)wall.gameObject.transform.localPosition.x,
                (int)wall.gameObject.transform.localPosition.y
            };
            wallData.LocalRotation = wall.gameObject.transform.localEulerAngles.z;
            wallData.Room = wall.Room;

            return wallData;
        }
    }
}