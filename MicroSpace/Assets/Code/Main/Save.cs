using System;
using System.Collections.Generic;
using Ships;
using UnityEngine;

namespace Main
{
    [Serializable]
    public class Save
    {
        #region Fields

        [SerializeField]
        private int _nextId;

        [SerializeField]
        private int _focusedShipId;

        [SerializeField]
        private List<SerializableShip> _ships = new();

        #endregion Fields

        #region Properties

        public List<SerializableShip> Ships => _ships;

        public int NextId => _nextId;

        public int FocusedShipId => _focusedShipId;

        #endregion Properties

        public Save(List<Ship> ships)
        {
            foreach (var ship in ships)
                _ships.Add(new(ship));
            _nextId = GameManager.Instance.IdManager.NextId - 1;
            _focusedShipId = GameManager.FocusedShipId;
        }
    }
}