using Assets.Code.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Data
{
    [Serializable]
    public class WallData
    {
        public float CurrentEndurance;
        public float[] LocalPosition = new float[2];
        public float LocalRotation;
        public float MaxEndurance;
        public string Name;
        public float Resilience;

        public static implicit operator WallData(Wall wall)
        {
            WallData wallData = new();
            wallData.Name = wall.Name;
            wallData.Resilience = wall.Resilience;
            wallData.MaxEndurance = wall.MaxEndurance;
            wallData.CurrentEndurance = wall.CurrentEndurance;
            wallData.LocalPosition = new float[]
            {
                wall.gameObject.transform.localPosition.x,
                wall.gameObject.transform.localPosition.y
            };
            wallData.LocalRotation = wall.gameObject.transform.localEulerAngles.z;

            return wallData;
        }
    }
}