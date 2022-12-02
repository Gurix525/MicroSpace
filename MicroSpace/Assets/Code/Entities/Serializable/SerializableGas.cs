using System;
using UnityEngine;

namespace Entities
{
    [Serializable]
    public struct SerializableGas
    {
        [field: SerializeField]
        public int ContainerId { get; private set; }

        [field: SerializeField]
        public int ModelId { get; private set; }

        [field: SerializeField]
        public int Amount { get; private set; }

        public SerializableGas(int containerId, int modelId, int amount)
        {
            ContainerId = containerId;
            ModelId = modelId;
            Amount = amount;
        }
    }
}