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
        public int ItemModel { get; private set; }

        [field: SerializeField, ReadonlyInspector]
        public int Id { get; private set; }

        public bool IsTool => this is Tool;

        public Item(int itemModel)
        {
            ItemModel = itemModel;
            Id = IdManager.NextId;
        }

        public Item(int itemModel, int id)
        {
            ItemModel = itemModel;
            Id = id;
        }
    }
}