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

        #endregion Private
    }
}