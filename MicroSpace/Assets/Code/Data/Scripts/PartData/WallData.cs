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
        public static implicit operator WallData(Wall wall)
        {
            var wallData = wall.WallData ?? new();
            //blockData.Name = block.Name;
            //blockData.Resilience = block.Resilience;
            //blockData.MaxEndurance = block.MaxEndurance;
            //blockData.CurrentEndurance = block.CurrentEndurance;
            wallData.LocalPosition = new int[]
            {
                (int)Math.Round( wall.gameObject.transform.localPosition.x),
                (int)Math.Round( wall.gameObject.transform.localPosition.y)
            };
            wallData.LocalRotation = wall.gameObject.transform.localEulerAngles.z;
            wall.WallData = wallData;
            return wallData;
        }
    }
}