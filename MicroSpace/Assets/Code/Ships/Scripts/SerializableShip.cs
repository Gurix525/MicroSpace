using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Code.Ships
{
    [Serializable]
    public class SerializableShip : MonoBehaviour
    {
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
    }
}