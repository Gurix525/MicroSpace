using Miscellaneous;
using UnityEngine;

namespace Entities
{
    public abstract class Entity : MonoBehaviour, IIdentifiable
    {
        public abstract int Id { get; protected set; }

        public virtual void SetId(int id)
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