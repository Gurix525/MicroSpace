using System;
using System.Collections.Generic;
using Ships;
using UnityEngine;

namespace Main
{
    [Serializable]
    public class Save
    {
        [SerializeField]
        private int _nextId;

        [SerializeField]
        private List<SerializableShip> _ships = new();

        public List<SerializableShip> Ships => _ships;

        public int NextId => _nextId;

        public Save(List<Ship> ships)
        {
            foreach (var ship in ships)
                _ships.Add(new(ship));
            _nextId = GameManager.Instance.IdManager.NextId - 1;
        }
    }
}