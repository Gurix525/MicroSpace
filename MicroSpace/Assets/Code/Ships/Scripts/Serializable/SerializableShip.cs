using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Assets.Code.Ships
{
    [Serializable]
    public class SerializableShip
    {
        #region Fields

        [SerializeField]
        private int _id;

        [SerializeField]
        private Vector2 _position;

        [SerializeField]
        private float _rotation;

        [SerializeField]
        private Vector2 _velocity;

        [SerializeField]
        private List<SerializableBlock> _blocks = new();

        [SerializeField]
        private List<Room> _rooms = new();

        #endregion Fields

        #region Properties

        public List<SerializableBlock> Blocks => _blocks;

        public List<SerializableWall> Walls => _blocks
            .Where(block => block is SerializableWall)
            .Select(block => block as SerializableWall)
            .ToList();

        public List<SerializableFloor> Floors => _blocks
            .Where(block => block is SerializableFloor)
            .Select(block => block as SerializableFloor)
            .ToList();

        public List<Room> Rooms => _rooms;

        public int Id => _id;

        public Vector2 Position => _position;

        public float Rotation => _rotation;

        public Vector2 Velocity => _velocity;

        #endregion Properties

        public SerializableShip(Ship ship)
        {
            foreach (var block in ship.Blocks)
            {
                if (block is Floor)
                    _blocks.Add(new SerializableFloor(block));
                else if (block is WallDesignation)
                    _blocks.Add(new SerializableWallDesignation(block));
                else if (block is FloorDesignation)
                    _blocks.Add(new SerializableFloorDesignation(block));
                else
                    _blocks.Add(new SerializableWall(block));
            }
            _rooms = ship.Rooms;
            _id = ship.Id;
            _position = ship.Position;
            _rotation = ship.Rotation;
            _velocity = ship.Velocity;
        }
    }
}