using Attributes;
using UnityEngine;

namespace Entities
{
    public class MassItem : Item
    {
        [field: SerializeField, ReadonlyInspector]
        public override int ModelId { get; set; }

        [field: SerializeField, ReadonlyInspector]
        public float Mass { get; set; }
    }
}