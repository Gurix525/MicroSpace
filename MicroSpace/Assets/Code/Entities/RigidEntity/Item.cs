using System.Collections.Generic;
using Attributes;
using UnityEngine;

namespace Entities
{
    public abstract class Item : RigidEntity
    {
        private ItemType _itemType;

        public abstract int ModelId { get; set; }

        [field: SerializeField, ReadonlyInspector]
        public bool IsOccupied { get; set; }

        public static List<Item> EnabledItems { get; } = new();

        public static List<MassItem> EnabledMassItems { get; } = new();

        public static List<SingleItem> EnabledSingleItems { get; } = new();

        public override void DestroySelf()
        {
            RemoveSelfFromList();
            base.DestroySelf();
        }

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
            AddSelfToList();
        }

        private void OnDisable()
        {
            RemoveSelfFromList();
        }

        private void AddSelfToList()
        {
            if (!EnabledItems.Contains(this))
                EnabledItems.Add(this);
            if (_itemType == ItemType.MassItem)
            {
                if (!EnabledMassItems.Contains((MassItem)this))
                    EnabledMassItems.Add((MassItem)this);
            }
            else
            {
                if (!EnabledSingleItems.Contains((SingleItem)this))
                    EnabledSingleItems.Add((SingleItem)this);
            }
        }

        private void RemoveSelfFromList()
        {
            if (EnabledItems.Contains(this))
                EnabledItems.Remove(this);
            if (_itemType == ItemType.MassItem)
            {
                if (EnabledMassItems.Contains((MassItem)this))
                    EnabledMassItems.Remove((MassItem)this);
            }
            else
            {
                if (EnabledSingleItems.Contains((SingleItem)this))
                    EnabledSingleItems.Remove((SingleItem)this);
            }
        }
    }
}