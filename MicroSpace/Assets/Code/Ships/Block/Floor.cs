using System;
using UnityEngine;
using Data;

namespace Ships
{
    [Serializable]
    public class Floor : Block
    {
        [SerializeField]
        private bool _isExposed = false;

        [SerializeField]
        private Floor _upFloor = null;

        [SerializeField]
        private Floor _downFloor = null;

        [SerializeField]
        private Floor _leftFloor = null;

        [SerializeField]
        private Floor _rightFloor = null;

        [SerializeField]
        private Room _room = null;

        public bool IsExposed { get => _isExposed; set => _isExposed = value; }
        public Floor UpFloor { get => _upFloor; set => _upFloor = value; }
        public Floor DownFloor { get => _downFloor; set => _downFloor = value; }
        public Floor LeftFloor { get => _leftFloor; set => _leftFloor = value; }
        public Floor RightFloor { get => _rightFloor; set => _rightFloor = value; }
        public Room Room { get => _room; set => _room = value; }

        private void Update()
        {
            // DEBUG
            // Poniższego ifa trzeba przenieść do ShipData.UpdateRooms()
            // i uogólnić, żeby się wszystkie sprawdzały od razu po sprawdzaniu
            // czy są exposed
            if (IsExposed)
                GetComponent<SpriteRenderer>().color = new Color32(127, 63, 63, 255);
            else
                GetComponent<SpriteRenderer>().color = new Color32(63, 63, 127, 255);
        }
    }
}