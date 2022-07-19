using Assets.Code.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Code.Data.PartDataImplementations;

namespace Assets.Code.Data
{
    [Serializable]
    public class ShipData
    {
        public List<BlockData> Blocks = new();
        public List<WallData> Walls = new();
        public List<RoomData> Rooms = new();

        private PartData[,] _grid;

        public void Update(GameObject ship)
        {
            UpdateBlocks(ship);
            UpdateWalls(ship);
            UpdateRooms();
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

        private PartData[,] CreateGrid()
        {
            List<PartData> parts = new();
            foreach (var item in Blocks)
                parts.Add(item);
            foreach (var item in Walls)
                parts.Add(item);
            int minX = (int)(parts.Min(x => x.LocalPosition[0]));
            int maxX = (int)(parts.Max(x => x.LocalPosition[0]));
            int minY = (int)(parts.Min(x => x.LocalPosition[1]));
            int maxY = (int)(parts.Max(x => x.LocalPosition[1]));
            int width = maxX - minX + 1;
            int height = maxY - minY + 1;
            PartData[,] grid = new PartData[width, height];
            foreach (var item in parts)
            {
                grid[
                    (int)item.LocalPosition[0] - minX,
                    (int)item.LocalPosition[1] - minY] = item;
            }
            return grid;
        }

        private void UpdateRooms()
        {
            var grid = CreateGrid();
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            if (width < 3 || height < 3)
                return;

            bool[,] boolGrid = new bool[width, height];
            for (int x = 1; x <= width - 2; x++)
                for (int y = 1; y <= height - 2; y++)
                    if (grid[x - 1, y] != null &&
                        grid[x + 1, y] != null &&
                        grid[x, y - 1] != null &&
                        grid[x, y + 1] != null &&
                        grid[x, y] is WallData)
                        boolGrid[x, y] = true;

            for (int x = 1; x <= width - 2; x++)
                for (int y = 1; y <= height - 2; y++)
                {
                    if (boolGrid[x, y])
                    {
                        var wall = grid[x, y] as WallData;
                        if (grid[x - 1, y] is WallData)
                            wall.Room = (grid[x - 1, y] as WallData).Room;
                        else if (grid[x, y - 1] is WallData)
                            wall.Room = (grid[x, y - 1] as WallData).Room;
                        else
                        {
                            Rooms.Add(new());
                            var room = Rooms[^1];
                            wall.Room = room;
                            room.Walls.Add(wall);
                            room.Id = Rooms.Count;
                        }
                    }
                } // ten algorytm jest zły, nie sprawdza czy pokój jest ograniczony blokami
            // Poza tym trzeba jeszcze się upewnić że roomy nie będą się tworzyć w nieskończonośc dla tych samych walli
            // No i z jakiegoś powodu na razie roomy w ogóle się nie tworzą
        }
    }
}