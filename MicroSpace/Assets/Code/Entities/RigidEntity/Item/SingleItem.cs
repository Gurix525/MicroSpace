using Inventory;
using UnityEngine;

namespace Entities
{
    public class SingleItem : Item
    {
        public override int ModelId
        {
            get
            {
                return Item.ModelId;
            }
            set { }
        }

        [field: SerializeField]
        public Inventory.Item Item { get; private set; }
    }
}