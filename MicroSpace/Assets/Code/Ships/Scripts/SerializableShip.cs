using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Code.Ships
{
    [Serializable]
    public class SerializableShip
    {
        [SerializeField]
        private int _id;

        [SerializeField]
        private Vector2 _position;

        [SerializeField]
        private float _rotation;

        [SerializeField]
        private Vector2 _velocity;

        [SerializeField]
        private List<SerializableWall> _walls = new();

        [SerializeField]
        private List<SerializableFloor> _floors = new();

        [SerializeField]
        private List<Room> _rooms = new();

        public List<SerializableWall> Walls => _walls;
        public List<SerializableFloor> Floors => _floors;
        public List<Room> Rooms => _rooms;
        public int Id => _id;
        public Vector2 Position => _position;
        public float Rotation => _rotation;
        public Vector2 Velocity => _velocity;

        public SerializableShip(Ship ship)
        {
            foreach (var wall in ship.Walls)
                _walls.Add(new(wall));
            foreach (var floor in ship.Floors)
                _floors.Add(new(floor));
            _rooms = ship.Rooms;
            _id = ship.Id;
            _position = ship.Position;
            _rotation = ship.Rotation;
            _velocity = ship.Velocity;
        }
    }
}