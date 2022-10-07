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
        public List<WallData> Walls = new();
        public List<FloorData> Floors = new();
        public List<RoomData> Rooms = new();

        public int ElementsCount { get => Floors.Count + Walls.Count; }

        private PartData[,] _grid;

        public void Update(GameObject ship)
        {
            UpdateWalls(ship);
            UpdateFloors(ship);
            UpdateRooms();
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

        private void UpdateFloors(GameObject ship)
        {
            List<FloorData> floors = new();

            foreach (Transform item in ship.transform)
            {
                var floor = item.gameObject.GetComponent<Floor>();
                if (floor != null)
                    floors.Add(floor);
            }

            Floors = floors;
        }

        private void UpdateRooms()
        {
            List<PartData> parts = new();
            foreach (var item in Walls)
                parts.Add(item);
            foreach (var item in Floors)
                parts.Add(item);

            foreach (PartData part in parts)
            {
                if (part is not FloorData)
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
                    ((FloorData)part).IsExposed = true;
                else
                    ((FloorData)part).IsExposed = false;

                if (upPart is FloorData)
                    ((FloorData)part).UpFloor = (FloorData)upPart;
                if (downPart is FloorData)
                    ((FloorData)part).DownFloor = (FloorData)downPart;
                if (leftPart is FloorData)
                    ((FloorData)part).LeftFloor = (FloorData)leftPart;
                if (rightPart is FloorData)
                    ((FloorData)part).RightFloor = (FloorData)rightPart;
            }

            var exposedFloors = parts
                .Where(x => x is FloorData)
                .Where(x => ((FloorData)x).IsExposed);

            foreach (var floor in exposedFloors)
                exposeFloor((FloorData)floor);

            foreach (var floor in Floors)
                setRoom(floor);

            void exposeFloor(FloorData floor)
            {
                floor.IsExposed = true;

                if (floor.UpFloor != null)
                    if (!floor.UpFloor.IsExposed)
                        exposeFloor(floor.UpFloor);

                if (floor.DownFloor != null)
                    if (!floor.DownFloor.IsExposed)
                        exposeFloor(floor.DownFloor);

                if (floor.LeftFloor != null)
                    if (!floor.LeftFloor.IsExposed)
                        exposeFloor(floor.LeftFloor);

                if (floor.RightFloor != null)
                    if (!floor.RightFloor.IsExposed)
                        exposeFloor(floor.RightFloor);
            }

            void setRoom(FloorData floor, RoomData room = null)
            {
                if (floor.Room.Id > 0)
                    room = floor.Room;
                else
                {
                    if (room == null)
                    {
                        Rooms.Add(new(Rooms.Count + 1));
                        room = Rooms[^1];
                    }
                    floor.Room = room;
                }

                if (floor.UpFloor != null)
                    if (floor.UpFloor.Room.Id == 0)
                        setRoom(floor.UpFloor, room);
                if (floor.DownFloor != null)
                    if (floor.DownFloor.Room.Id == 0)
                        setRoom(floor.DownFloor, room);
                if (floor.LeftFloor != null)
                    if (floor.LeftFloor.Room.Id == 0)
                        setRoom(floor.LeftFloor, room);
                if (floor.RightFloor != null)
                    if (floor.RightFloor.Room.Id == 0)
                        setRoom(floor.RightFloor, room);
                //void setRoom(WallData wall, RoomData room = null)
                //{
                //    if (wall == null)
                //        return;

                //    if (wall.Room != null)
                //        if (wall.Room.Id != 0)
                //            return;

                //    if (room == null)
                //    {
                //        Rooms.Add(new(Rooms.Count + 1));
                //        room = Rooms[^1];
                //    }

                //    if (wall.Room == null)
                //    {
                //        room.Walls.Add(wall);
                //        wall.Room = room;
                //    }

                //    setRoom(wall.UpWall, room);
                //    setRoom(wall.DownWall, room);
                //    setRoom(wall.LeftWall, room);
                //    setRoom(wall.RightWall, room);
                //}
            } // Jeszcze sie za duzo roomow tworzy bo

            // walle sie kopiują z pustymi roomami na WallData
        }
    }
}