using ExtensionMethods;
using Miscellaneous;
using ScriptableObjects;
using UnityEngine;

namespace Entities
{
    public abstract class SolidBlock : Block
    {
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
            foreach (var item in BlockModel.GetModel(ModelId).Items)
            {
                var newItem = Instantiate(
                    Prefabs.MassItem,
                    transform.position,
                    transform.rotation,
                    References.WorldTransform)
                    .GetComponent<MassItem>();
                newItem.ModelId = item.Key.Id;
                newItem.Mass = item.Value;
                newItem.GetComponent<Rigidbody2D>().velocity = velocity;
            }
        }
    }
}