using ExtensionMethods;
using Miscellaneous;
using ScriptableObjects;
using UnityEngine;

namespace Entities
{
    public abstract class SolidBlock : Block
    {
        private Satellite _satellite;
        private Rigidbody2D _rigidbody;

        private Satellite Satellite =>
            _satellite ??= this.GetComponentUpInHierarchy<Satellite>();

        private Rigidbody2D Rigidbody =>
            _rigidbody ??= Satellite.GetComponent<Rigidbody2D>();

        public void ExecuteMiningTask()
        {
            GetComponent<Collider2D>().enabled = false;
            Rigidbody2D rigidbody = Rigidbody;
            transform.parent = null;
            Satellite.UpdateSatellite();
            SpawnMassItems(rigidbody.velocity);
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
                    References.World)
                    .GetComponent<MassItem>();
                newItem.ModelId = item.Key.Id;
                newItem.Mass = item.Value;
                newItem.GetComponent<Rigidbody2D>().velocity = velocity;
            }
        }
    }
}