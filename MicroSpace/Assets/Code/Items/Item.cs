using System;
using Attributes;
using Miscellaneous;
using UnityEngine;

namespace Items
{
    [Serializable]
    public class Item : IIdentifiable
    {
        [field: SerializeField, ReadonlyInspector]
        public int ModelId { get; private set; }

        [field: SerializeField, ReadonlyInspector]
        public int Id { get; private set; }

        public bool IsTool => this is Tool;

        public Item(int modelId)
        {
            ModelId = modelId;
            Id = IdManager.NextId;
        }

        public Item(int modelId, int id)
        {
            ModelId = modelId;
            Id = id;
        }
    }
}