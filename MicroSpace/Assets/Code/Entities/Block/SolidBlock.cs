using System;
using ExtensionMethods;
using Miscellaneous;
using ScriptableObjects;
using UnityEngine;

namespace Entities
{
    public abstract class SolidBlock : Block
    {
        [SerializeField]
        private GameObject _light;

        private Satellite _satellite;
        private Rigidbody2D _rigidbody;

        public Maths.Range ShadowRange
        {
            get
            {
                Vector2 perpendicular = Vector2
                    .Perpendicular(transform.position).normalized;
                float rotation = Math.Abs((transform.eulerAngles.z
                    + Vector2.SignedAngle(Vector2.up, transform.position)) % 90);
                float scaledRotation = rotation < 45 ? rotation : 90 - rotation;
                float oneSideWidth = 0.5F + scaledRotation
                    / 45 * 0.41F / 2;
                float leftAngle = Vector2.SignedAngle(
                    Vector2.up,
                    (Vector2)transform.position - perpendicular * oneSideWidth);
                float rightAngle = Vector2.SignedAngle(
                    Vector2.up,
                    (Vector2)transform.position + perpendicular * oneSideWidth);
                return new Maths.Range(leftAngle, rightAngle);
            }
        }

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

        public void SetLightActive(bool state)
        {
            _light.SetActive(state);
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