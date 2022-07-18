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
        public List<BlockData> Blocks = new();
        public List<WallData> Walls = new();
        public List<RoomData> Rooms = new();

        public void Update(GameObject ship)
        {
            UpdateBlocks(ship);
            UpdateWalls(ship);
        }

        private void UpdateBlocks(GameObject ship)
        {
            List<BlockData> blocks = new();

            foreach (Transform item in ship.transform)
            {
                var block = item.gameObject.GetComponent<Block>();
                if (block != null)
                    blocks.Add(block);
            }

            Blocks = blocks;
        }

        private void UpdateWalls(GameObject ship)
        {
            List<WallData> walls = new();

            foreach (Transform item in ship.transform)
            {
                var wall = item.gameObject.GetComponent<Wall>();
                if (wall != null)
                    walls.Add(wall);
            }

            Walls = walls;
        }
    }
}