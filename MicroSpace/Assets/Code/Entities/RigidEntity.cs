using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public abstract class RigidEntity : Entity
    {
        #region Fields

        [SerializeField]
        private GameObject _light;

        private Rigidbody2D _rigidbody;
        private int _lightTimer = 0;

        #endregion Fields

        #region Properties

        public Rigidbody2D Rigidbody =>
            _rigidbody ??= GetComponent<Rigidbody2D>();

        public Maths.Range ShadowRange
        {
            get
            {
                Vector2 perpendicular = Vector2
                    .Perpendicular(transform.position).normalized;
                float oneSideWidth = transform.localScale.x / 2;
                float leftAngle = Vector2.SignedAngle(
                    Vector2.up,
                    (Vector2)transform.position - perpendicular * oneSideWidth);
                float rightAngle = Vector2.SignedAngle(
                    Vector2.up,
                    (Vector2)transform.position + perpendicular * oneSideWidth);
                return new Maths.Range(leftAngle, rightAngle);
            }
        }

        public static List<RigidEntity> RigidEntities { get; } = new();

        public static List<RigidEntity> EnabledRigidEntities { get; } = new();

        #endregion Properties

        #region Public

        public virtual void DestroySelf()
        {
            SetAstronautsFree();
            Destroy(gameObject);
        }

        public void SetLightActive(bool state)
        {
            if (_lightTimer < 10 || _light.activeInHierarchy == state)
                return;
            _lightTimer = 0;
            _light.SetActive(state);
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
            AddSelfToList();
        }

        protected virtual void OnEnable()
        {
            AddSelfToList();
        }

        protected virtual void FixedUpdate()
        {
            _lightTimer++;
        }

        protected virtual void OnDisable()
        {
            RemoveSelfFromList();
        }

        private void OnDestroy()
        {
            RemoveSelfFromList();
        }

        #endregion Unity

        #region Private

        private void SetAstronautsFree()
        {
            var astronauts = GetComponentsInChildren<Astronaut>();
            foreach (var astronaut in astronauts)
            {
                astronaut.transform.parent = null;
            }
        }

        private void AddSelfToList()
        {
            if (!RigidEntities.Contains(this))
                RigidEntities.Add(this);
            if (!EnabledRigidEntities.Contains(this))
                EnabledRigidEntities.Add(this);
        }

        private void RemoveSelfFromList()
        {
            if (RigidEntities.Contains(this))
                RigidEntities.Remove(this);
            if (EnabledRigidEntities.Contains(this))
                EnabledRigidEntities.Remove(this);
        }

        #endregion Private
    }
}