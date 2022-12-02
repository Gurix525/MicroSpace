using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.ObjectModel;

namespace Entities
{
    [Serializable]
    public class SerializableSatellite
    {
        #region Properties

        [field: SerializeField]
        public List<SerializableBlock> Blocks { get; private set; } = new();

        [field: SerializeField]
        public List<SerializableGas> Gasses { get; private set; } = new();

        [field: SerializeField]
        public int Id { get; private set; }

        [field: SerializeField]
        public Vector2 Position { get; private set; }

        [field: SerializeField]
        public float Rotation { get; private set; }

        [field: SerializeField]
        public Vector2 Velocity { get; private set; }

        #endregion Properties

        public SerializableSatellite(Satellite satellite)
        {
            foreach (var block in satellite.Blocks)
                Blocks.Add(new(block));
            foreach (var block in satellite.Blocks)
                if (block is IGasContainer)
                    AddGasses((IGasContainer)block);
            Id = satellite.Id;
            Position = satellite.Position;
            Rotation = satellite.Rotation;
            Velocity = satellite.Velocity;
        }

        public static implicit operator SerializableSatellite(Satellite satellite)
        {
            return new SerializableSatellite(satellite);
        }

        private void AddGasses(IGasContainer container)
        {
            foreach (var gas in container.Gasses)
            {
                Gasses.Add(new(container.Id, gas.Key, gas.Value));
            }
        }
    }
}