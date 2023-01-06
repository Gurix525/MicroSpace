using ExtensionMethods;
using Miscellaneous;
using ScriptableObjects;
using UnityEngine;

namespace Entities
{
    public abstract class SolidBlock : Block
    {
        [SerializeField]
        private MassItem _massItemPrefab;

        public void ExecuteMiningTask()
        {
            GetComponent<Collider2D>().enabled = false;
            Satellite satellite = this.GetComponentUpInHierarchy<Satellite>();
            transform.parent = null;
            satellite.UpdateSatellite();
            SpawnMassItems(satellite.GetComponent<Rigidbody2D>().velocity);
            Destroy(gameObject);
        }

        private void SpawnMassItems(Vector2 velocity)
        {
            var items = BlockModel.GetModel(ModelId).Items;
            foreach (var item in items)
            {
                var newItem = Instantiate(
                    _massItemPrefab,
                    transform.position,
                    transform.rotation,
                    References.WorldTransform);
                newItem.ItemModel = item.Key.Id;
                newItem.Mass = item.Value;
                newItem.GetComponent<Rigidbody2D>().velocity = velocity;
            }
        }
    }
}