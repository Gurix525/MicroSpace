using System;
using UnityEngine;
using Attributes;

namespace Entities
{
    [Serializable]
    public class Floor : SolidBlock
    {
        [SerializeField]
        [ReadonlyInspector]
        private bool _isExposed = false;

        [SerializeField]
        [ReadonlyInspector]
        private Floor _upFloor = null;

        [SerializeField]
        [ReadonlyInspector]
        private Floor _downFloor = null;

        [SerializeField]
        [ReadonlyInspector]
        private Floor _leftFloor = null;

        [SerializeField]
        [ReadonlyInspector]
        private Floor _rightFloor = null;

        [SerializeField]
        [ReadonlyInspector]
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
            // Poniższego ifa trzeba przenieść do SatelliteData.UpdateRooms()
            // i uogólnić, żeby się wszystkie sprawdzały od razu po sprawdzaniu
            // czy są exposed
            if (IsExposed)
                GetComponent<SpriteRenderer>().color = new Color32(127, 63, 63, 255);
            else
                GetComponent<SpriteRenderer>().color = new Color32(63, 63, 127, 255);
        }
    }
}