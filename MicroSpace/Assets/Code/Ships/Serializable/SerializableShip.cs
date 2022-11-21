using System.Collections.Generic;
using UnityEngine;
using System;

namespace Ships
{
    [Serializable]
    public class SerializableShip
    {
        #region Fields

        // Pola nie mogą być readonly bo serializacja nie zadziała

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

        public List<Room> Rooms => _rooms;

        public int Id => _id;

        public Vector2 Position => _position;

        public float Rotation => _rotation;

        public Vector2 Velocity => _velocity;

        #endregion Properties

        public SerializableShip(Ship ship)
        {
            foreach (var block in ship.Blocks)
                _blocks.Add(new(block));
            _rooms = ship.Rooms;
            _id = ship.Id;
            _position = ship.Position;
            _rotation = ship.Rotation;
            _velocity = ship.Velocity;
        }

        public static implicit operator SerializableShip(Ship ship)
        {
            return new SerializableShip(ship);
        }
    }
}