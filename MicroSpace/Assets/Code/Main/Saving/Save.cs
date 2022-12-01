using System;
using System.Collections.Generic;
using Entities;
using UnityEngine;
using System.Linq;
using Miscellaneous;

namespace Main
{
    [Serializable]
    public class Save
    {
        #region Fields

        [SerializeField]
        private int _nextId;

        [SerializeField]
        private int _focusedSatelliteId;

        [SerializeField]
        private SerializableSatellite[] _satellites;

        [SerializeField]
        private SerializableAstronaut[] _astronauts;

        #endregion Fields

        #region Properties

        public List<SerializableSatellite> Satellites => _satellites.ToList();

        public List<SerializableAstronaut> Astronauts => _astronauts.ToList();

        public int NextId => _nextId;

        public int FocusedSatelliteId => _focusedSatelliteId;

        #endregion Properties

        public Save()
        {
            foreach (Satellite satellite in Satellite.Satellites)
                satellite.UpdateSatellite();
            _satellites = Satellite.Satellites
                .Select(satellite => (SerializableSatellite)satellite)
                .ToArray();
            _astronauts = Astronaut.Astronauts
                .Select(astronaut => (SerializableAstronaut)astronaut)
                .ToArray();
            _nextId = IdManager.NextId;
            _focusedSatelliteId = GameManager.FocusedSatelliteId;
        }
    }
}