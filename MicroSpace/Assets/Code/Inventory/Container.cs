using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class Container : MonoBehaviour
    {
        [field: SerializeField]
        public List<ContainerItem> ContainerItems { get; private set; } = new();

        public void AddItem(int modelId, float mass)
        {
            var containerItem = ContainerItems
                .Find(containerItem => containerItem.ModelId == modelId);
            if (containerItem == null)
                ContainerItems.Add(new(modelId, mass));
            else
                containerItem.Mass += mass;
        }

        public void RemoveItem(int modelId, float mass)
        {
            var containerItem = ContainerItems
                .Find(containerItem => containerItem.ModelId == modelId);
            if (containerItem == null)
                return;
            else
                containerItem.Mass -= mass;
            if (containerItem.Mass <= 0)
                ContainerItems.Remove(containerItem);
        }

        public bool HasItem(int modelId, float mass)
        {
            var containerItem = ContainerItems
                .Find(containerItem => containerItem.ModelId == modelId);
            if (containerItem != null)
                if (containerItem.Mass >= mass)
                    return true;
            return false;
        }
    }
}