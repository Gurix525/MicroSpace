using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.ObjectModel;

namespace Entities
{
    [Serializable]
    public class SerializableSatellite
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
        private List<SerializableGas> _gasses = new();

        #endregion Fields

        #region Properties

        public IEnumerable<SerializableBlock> Blocks => _blocks;

        public IEnumerable<SerializableGas> Gasses => _gasses;

        public int Id => _id;

        public Vector2 Position => _position;

        public float Rotation => _rotation;

        public Vector2 Velocity => _velocity;

        #endregion Properties

        public SerializableSatellite(Satellite satellite)
        {
            foreach (var block in satellite.Blocks)
                _blocks.Add(new(block));
            foreach (var block in satellite.Blocks)
                if (block is IGasContainer)
                    AddGasses((IGasContainer)block);
            _id = satellite.Id;
            _position = satellite.Position;
            _rotation = satellite.Rotation;
            _velocity = satellite.Velocity;
        }

        public static implicit operator SerializableSatellite(Satellite satellite)
        {
            return new SerializableSatellite(satellite);
        }

        private void AddGasses(IGasContainer container)
        {
            foreach (var gas in container.Gasses)
            {
                _gasses.Add(new(container.Id, gas.Key, gas.Value));
            }
        }
    }
}