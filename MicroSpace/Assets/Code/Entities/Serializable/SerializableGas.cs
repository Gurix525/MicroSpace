using System;
using UnityEngine;

namespace Entities
{
    [Serializable]
    public struct SerializableGas
    {
        [SerializeField]
        private int _containerId;

        [SerializeField]
        private int _modelId;

        [SerializeField]
        private int _amount;

        public int ContainerId => _containerId;

        public int ModelId => _modelId;

        public int Amount => _amount;

        public SerializableGas(int containerId, int modelId, int amount)
        {
            _containerId = containerId;
            _modelId = modelId;
            _amount = amount;
        }
    }
}