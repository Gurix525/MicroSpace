using System;
using System.Collections.Generic;
using ExtensionMethods;
using Maths;
using Miscellaneous;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Profiling;

namespace Entities
{
    public abstract class SolidBlock : Block
    {
        #region Fields

        private Rigidbody2D _rigidbody;

        #endregion Fields

        #region Properties

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

        #endregion Public

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