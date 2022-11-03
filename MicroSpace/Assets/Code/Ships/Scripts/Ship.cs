using UnityEngine;
using System.Collections.Generic;
using Assets.Code.Data;
using System.Linq;
using System;

namespace Assets.Code.Ships
{
    [Serializable]
    public class Ship : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private List<Wall> _walls = new();

        [SerializeField]
        private List<Floor> _floors = new();

        [SerializeField]
        private List<Room> _rooms = new();

        [SerializeField]
        private int _id;

        [SerializeField]
        private Vector2 _position;

        [SerializeField]
        private float _rotation;

        [SerializeField]
        private Vector2 _velocity;

        [SerializeField]
        private float _shipUnloadDistance = 100F;

        private bool _isShipLoaded = true;

        #endregion Fields

        #region Properties

        public List<Wall> Walls { get => _walls; set => _walls = value; }
        public List<Floor> Floors { get => _floors; set => _floors = value; }
        public List<Room> Rooms { get => _rooms; set => _rooms = value; }
        public int ElementsCount { get => Floors.Count + Walls.Count; }
        public int Id { get => _id; set => _id = value; }
        public Vector2 Position { get => _position; set => _position = value; }
        public float Rotation { get => _rotation; set => _rotation = value; }
        public Vector2 Velocity { get => _velocity; set => _velocity = value; }

        #endregion Properties

        #region Public

        public void UpdateShip()
        {
            UpdateWalls();
            UpdateFloors();
            UpdateRooms();
        }

        #endregion Public

        #region Private

        private void FixedUpdate()
        {
            SetChildrenActiveOrInactive();
        }

        private void SetChildrenActiveOrInactive()
        {
            if (IsShipDistantFromOrigin() && _isShipLoaded)
                SetShipChildrenActive(false);
            else if (!IsShipDistantFromOrigin() && !_isShipLoaded)
                SetShipChildrenActive(true);
        }

        private void SetShipChildrenActive(bool state)
        {
            foreach (Transform child in transform)
                child.gameObject.SetActive(state);
            _isShipLoaded = state;
        }

        private bool IsShipDistantFromOrigin()
        {
            return Vector2.Distance(transform.position, Vector2.zero) >
                _shipUnloadDistance;
        }

        private void UpdateWalls()
        {
            Walls.Clear();
            foreach (Transform item in transform)
            {
                var wall = item.gameObject.GetComponent<Wall>();
                if (wall != null)
                    Walls.Add(wall);
            }
        }

        private void UpdateFloors()
        {
            Floors.Clear();
            foreach (Transform item in transform)
            {
                var floor = item.gameObject.GetComponent<Floor>();
                if (floor != null)
                    Floors.Add(floor);
            }
        }

        private void UpdateRooms()
        {
            List<Block> blocks = new();
            foreach (var item in Walls)
                blocks.Add(item);
            foreach (var item in Floors)
                blocks.Add(item);

            foreach (Block block in blocks)
            {
                if (block is not Floor)
                    continue;

                var upBlock = blocks.Find(
                    x => x.transform.localPosition.x == block.transform.localPosition.x &&
                    x.transform.localPosition.y == block.transform.localPosition.y + 1);
                var downBlock = blocks.Find(
                    x => x.transform.localPosition.x == block.transform.localPosition.x &&
                    x.transform.localPosition.y == block.transform.localPosition.y - 1);
                var leftBlock = blocks.Find(
                    x => x.transform.localPosition.x == block.transform.localPosition.x - 1 &&
                    x.transform.localPosition.y == block.transform.localPosition.y);
                var rightBlock = blocks.Find(
                    x => x.transform.localPosition.x == block.transform.localPosition.x + 1 &&
                    x.transform.localPosition.y == block.transform.localPosition.y);

                if (upBlock == null || downBlock == null ||
                    leftBlock == null || rightBlock == null)
                    ((Floor)block).IsExposed = true;
                else
                    ((Floor)block).IsExposed = false;

                if (upBlock is Floor)
                    ((Floor)block).UpFloor = (Floor)upBlock;
                if (downBlock is Floor)
                    ((Floor)block).DownFloor = (Floor)downBlock;
                if (leftBlock is Floor)
                    ((Floor)block).LeftFloor = (Floor)leftBlock;
                if (rightBlock is Floor)
                    ((Floor)block).RightFloor = (Floor)rightBlock;
            }

            var exposedFloors = blocks
                .Where(x => x is Floor)
                .Where(x => ((Floor)x).IsExposed);

            foreach (var floor in exposedFloors)
                exposeFloor((Floor)floor);

            foreach (var floor in Floors)
                setRoom(floor);

            void exposeFloor(Floor floor)
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

            void setRoom(Floor floor, Room room = null)
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
            } // Jeszcze sie za duzo roomow tworzy bo

            // walle sie kopiują z pustymi roomami na WallData
        }

        #endregion Private
    }
}