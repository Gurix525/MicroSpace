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
            System.Random random = new();
            Vector2 randomizedPosition = transform.position + new Vector3(
                (float)random.NextDouble() * 0.6F - 0.3F,
                (float)random.NextDouble() * 0.6F - 0.3F,
                0);
            foreach (var item in items)
            {
                var newItem = Instantiate(
                    _massItemPrefab,
                    randomizedPosition,
                    transform.rotation,
                    References.WorldTransform);
                newItem.ModelId = item.Key.Id;
                newItem.Mass = item.Value;
                newItem.GetComponent<Rigidbody2D>().velocity = velocity;
            }
        }
    }
}