using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public abstract class RigidEntity : Entity
    {
        #region Fields

        private Rigidbody2D _rigidbody;

        #endregion Fields

        #region Properties

        public Rigidbody2D Rigidbody =>
            _rigidbody ??= GetComponent<Rigidbody2D>();

        public static List<RigidEntity> RigidEntities { get; } = new();

        public static List<RigidEntity> EnabledRigidEntities { get; } = new();

        #endregion Properties

        #region Public

        public void DestroyRigidEntity()
        {
            Destroy(gameObject);
        }

        public static void ForEach(Action<RigidEntity> action)
        {
            foreach (RigidEntity rigidEntity in RigidEntities)
                action(rigidEntity);
        }

        #endregion Public

        #region Unity

        protected override void Awake()
        {
            base.Awake();
            AddRigidEntityToList();
        }

        private void OnDestroy()
        {
            SetAstronautsFree();
            RemoveRigidEntityFromList();
        }

        #endregion Unity

        #region Private

        private void AddRigidEntityToList()
        {
            RigidEntities.Add(this);
        }

        private void RemoveRigidEntityFromList()
        {
            RigidEntities.Remove(this);
        }

        private void SetAstronautsFree()
        {
            var astronauts = GetComponentsInChildren<Astronaut>();
            foreach (var astronaut in astronauts)
            {
                astronaut.gameObject.SetActive(true);
                astronaut.transform.parent = null;
            }
        }

        #endregion Private
    }
}