using Attributes;
using Items;
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
        public Items.Item Item { get; private set; }
    }
}