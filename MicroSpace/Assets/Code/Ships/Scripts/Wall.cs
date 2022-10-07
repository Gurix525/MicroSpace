using Assets.Code.Data.PartDataImplementations;
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
    }
}