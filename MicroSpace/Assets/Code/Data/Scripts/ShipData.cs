using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Data
{
    [Serializable]
    public class ShipData
    {
        public List<WallData> Walls = new();
        public List<FloorData> Floors = new();
        public List<RoomData> Rooms = new();

        public int ElementsCount { get => Floors.Count + Walls.Count; }

        private PartData[,] _grid;
    }
}