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
        #region Properties

        [field: SerializeField]
        public List<SerializableSatellite> Satellites { get; private set; } = new();

        [field: SerializeField]
        public List<SerializableAstronaut> Astronauts { get; private set; } = new();

        [field: SerializeField]
        public int NextId { get; private set; }

        [field: SerializeField]
        public int FocusedSatelliteId { get; private set; }

        #endregion Properties

        public Save()
        {
            Satellites = Satellite.Satellites
                .Select(satellite => (SerializableSatellite)satellite)
                .ToList();
            Astronauts = Astronaut.Astronauts
                .Select(astronaut => (SerializableAstronaut)astronaut)
                .ToList();
            NextId = IdManager.NextId;
            FocusedSatelliteId = References.FocusedSatellite != null ?
                References.FocusedSatellite.GetComponent<Satellite>().Id :
                0;
        }
    }
}