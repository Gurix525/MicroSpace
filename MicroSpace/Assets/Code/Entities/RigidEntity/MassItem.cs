using Attributes;
using UnityEngine;

namespace Entities
{
    public class MassItem : RigidEntity
    {
        [field: SerializeField, ReadonlyInspector]
        public int ItemModel { get; set; }

        [field: SerializeField, ReadonlyInspector]
        public float Mass { get; set; }
    }
}