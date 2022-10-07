using Assets.Code.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Ships
{
    /// <summary>
    /// Ma swój odpowiednik w Data.WallData.
    /// </summary>
    public class Wall : Block
    {
        [SerializeField]
        private WallData _wallData;

        public WallData WallData { get => _wallData; set => _wallData = value; }

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