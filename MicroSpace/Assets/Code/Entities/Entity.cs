using Attributes;
using Miscellaneous;
using UnityEngine;

namespace Entities
{
    public abstract class Entity : MonoBehaviour, IIdentifiable
    {
        [field: SerializeField, ReadonlyInspector]
        public int Id { get; private set; }

        public void SetId(int id)
        {
            Id = id;
        }

        protected virtual void Awake()
        {
            CreateId();
        }

        private void CreateId()
        {
            if (Id == 0)
                Id = IdManager.NextId;
        }
    }
}