using Attributes;
using Items;
using UnityEngine;

namespace Entities
{
    public class SingleItem : RigidEntity
    {
        [field: SerializeField]
        public Item Item { get; private set; }
    }
}