using System.Collections.Generic;

namespace Entities
{
    public abstract class Item : RigidEntity
    {
        public abstract int ModelId { get; set; }

        public static List<Item> EnabledItems = new();

        protected void OnEnable()
        {
            EnabledItems.Add(this);
        }

        private void OnDisable()
        {
            EnabledItems.Remove(this);
        }
    }
}