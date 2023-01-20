using System;
using ExtensionMethods;
using Miscellaneous;
using ScriptableObjects;
using UnityEngine;

namespace Entities
{
    public abstract class SolidBlock : Block
    {
        #region Fields

        private Rigidbody2D _rigidbody;
        private int _lightTimer = 0;
        private bool _isSurface = false;
        private float _multiplier = 1.1F;

        #endregion Fields

        #region Properties

        public Maths.Range ShadowRange
        {
            get
            {
                float rightAngle = 0F;
                float leftAngle = 0F;
                float currentMultiplier = 1F;
                while (rightAngle == 0F || leftAngle == 0F)
                {
                    Vector2 perpendicular = Vector2
                    .Perpendicular(transform.position).normalized;
                    float rotation = Math.Abs((transform.eulerAngles.z
                        + Vector2.SignedAngle(Vector2.up, transform.position)) % 90);
                    float scaledRotation = rotation < 45 ? rotation : 90 - rotation;
                    float oneSideWidth = 0.6F + scaledRotation
                        / 45 * 0.5F / 2;
                    leftAngle = Vector2.SignedAngle(
                        Vector2.up * 10000F,
                        (Vector2)transform.position - perpendicular * oneSideWidth * currentMultiplier);
                    rightAngle = Vector2.SignedAngle(
                        Vector2.up * 10000F,
                        (Vector2)transform.position + perpendicular * oneSideWidth * currentMultiplier);
                    currentMultiplier *= _multiplier;
                }
                return new Maths.Range(leftAngle, rightAngle);
            }
        }

        public bool IsSurface => _isSurface;

        private Rigidbody2D Rigidbody =>
            _rigidbody ??= _satellite.GetComponent<Rigidbody2D>();

        #endregion Properties

        #region Public

        public void ExecuteMiningTask()
        {
            GetComponent<Collider2D>().enabled = false;
            Rigidbody2D rigidbody = Rigidbody;
            transform.parent = null;
            SpawnMassItems(rigidbody.velocity);
            Destroy(gameObject);
        }

        public void SetEnlighted(bool state)
        {
            if (_lightTimer < 10 || _isSurface == state)
                return;
            _lightTimer = 0;
            _isSurface = state;
        }

        #endregion Public

        #region Unity

        protected virtual void FixedUpdate()
        {
            _lightTimer++;
        }

        #endregion Unity

        #region Private

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

        #endregion Private
    }
}