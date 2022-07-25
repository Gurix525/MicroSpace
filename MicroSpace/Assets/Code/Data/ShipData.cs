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

        private void UpdateRooms()
        {
            List<PartData> parts = new();
            foreach (var item in Blocks)
                parts.Add(item);
            foreach (var item in Walls)
                parts.Add(item);

            foreach (PartData part in parts)
            {
                if (part is not WallData)
                    continue;

                var upPart = parts.Find(
                    x => x.LocalPosition[0] == part.LocalPosition[0] &&
                    x.LocalPosition[1] == part.LocalPosition[1] + 1);
                var downPart = parts.Find(
                    x => x.LocalPosition[0] == part.LocalPosition[0] &&
                    x.LocalPosition[1] == part.LocalPosition[1] - 1);
                var leftPart = parts.Find(
                    x => x.LocalPosition[0] == part.LocalPosition[0] - 1 &&
                    x.LocalPosition[1] == part.LocalPosition[1]);
                var rightPart = parts.Find(
                    x => x.LocalPosition[0] == part.LocalPosition[0] + 1 &&
                    x.LocalPosition[1] == part.LocalPosition[1]);

                if (upPart == null || downPart == null ||
                    leftPart == null || rightPart == null)
                    ((WallData)part).IsExposed = true;

                if (upPart is WallData)
                    ((WallData)part).UpWall = (WallData)upPart;
                if (downPart is WallData)
                    ((WallData)part).DownWall = (WallData)downPart;
                if (leftPart is WallData)
                    ((WallData)part).LeftWall = (WallData)leftPart;
                if (rightPart is WallData)
                    ((WallData)part).RightWall = (WallData)rightPart;
            }

            var exposedWalls = parts
                .Where(x => x is WallData)
                .Where(x => ((WallData)x).IsExposed);

            foreach (var wall in exposedWalls)
                ExposeWall((WallData)wall);

            foreach (var wall in Walls)
                SetRoom(wall);

            void ExposeWall(WallData wall)
            {
                wall.IsExposed = true;

                if (wall.UpWall != null)
                    if (!wall.UpWall.IsExposed)
                        ExposeWall(wall.UpWall);

                if (wall.DownWall != null)
                    if (!wall.DownWall.IsExposed)
                        ExposeWall(wall.DownWall);

                if (wall.LeftWall != null)
                    if (!wall.LeftWall.IsExposed)
                        ExposeWall(wall.LeftWall);

                if (wall.RightWall != null)
                    if (!wall.RightWall.IsExposed)
                        ExposeWall(wall.RightWall);
            }

            void SetRoom(WallData wall, RoomData room = null)
            {
                if (wall == null)
                    return;

                if (wall.Room != null)
                    return;

                if (room == null)
                {
                    Rooms.Add(new(Rooms.Count));
                    room = Rooms[^1];
                }

                if (wall.Room == null)
                {
                    room.Walls.Add(wall);
                    wall.Room = room;
                }

                SetRoom(wall.UpWall, room);
                SetRoom(wall.DownWall, room);
                SetRoom(wall.LeftWall, room);
                SetRoom(wall.RightWall, room);
            }
        } // Jeszcze sie za duzo roommow tworzy z jakiegoś powodu
    }
}