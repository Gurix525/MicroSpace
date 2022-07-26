using Assets.Code.Data.PartDataImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Data
{
    [Serializable]
    public class RoomData
    {
        public int Id;

        public float MaxPressure;

        public float Pressure;
        [NonSerialized] public List<WallData> Walls = new();

        public RoomData(int id)
        {
            Id = id;
        }
    }
}