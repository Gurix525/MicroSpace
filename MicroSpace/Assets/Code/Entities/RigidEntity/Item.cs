using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public abstract class Item : RigidEntity
    {
        private ItemType _itemType;

        public abstract int ModelId { get; set; }

        public static List<Item> EnabledItems { get; } = new();

        public static List<MassItem> EnabledMassItems { get; } = new();

        public static List<SingleItem> EnabledSingleItems { get; } = new();

        protected override void Awake()
        {
            base.Awake();
            System.Random random = new();
            transform.position += new Vector3(
                (float)random.NextDouble() * 0.6F - 0.3F,
                (float)random.NextDouble() * 0.6F - 0.3F,
                0);
            _itemType = this is MassItem ?
                ItemType.MassItem :
                ItemType.SingleItem;
        }

        protected void OnEnable()
        {
            EnabledItems.Add(this);
            if (_itemType == ItemType.MassItem)
                EnabledMassItems.Add((MassItem)this);
            else
                EnabledSingleItems.Add((SingleItem)this);
        }

        private void OnDisable()
        {
            EnabledItems.Remove(this);
            if (_itemType == ItemType.MassItem)
                EnabledMassItems.Remove((MassItem)this);
            else
                EnabledSingleItems.Remove((SingleItem)this);
        }
    }
}