using Assets.Code.Ships;
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

        public void Update(GameObject ship)
        {
            UpdateWalls(ship);
        }

        private void UpdateWalls(GameObject ship)
        {
            List<WallData> walls = new();

            foreach (Transform item in ship.transform)
            {
                var wall = item.gameObject.GetComponent<Wall>();
                if (wall != null)
                {
                    walls.Add(wall);
                }
            }

            Walls = walls;
        }
    }
}