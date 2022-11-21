using System;
using System.Collections.Generic;
using Ships;
using UnityEngine;
using ScriptableObjects;
using System.Linq;

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
        private SerializableShip[] _ships;

        [SerializeField]
        private SerializableAstronaut[] _astronauts;

        #endregion Fields

        #region Properties

        public List<SerializableShip> Ships => _ships.ToList();

        public List<SerializableAstronaut> Astronauts => _astronauts.ToList();

        public int NextId => _nextId;

        public int FocusedShipId => _focusedShipId;

        #endregion Properties

        public Save(List<Ship> ships)
        {
            _ships = ships
                .Select(ship => (SerializableShip)ship)
                .ToArray();
            _astronauts = Astronaut.Astronauts
                .Select(astronaut => (SerializableAstronaut)astronaut)
                .ToArray();
            _nextId = IdManager.NextId;
            _focusedShipId = GameManager.FocusedShipId;
        }
    }
}