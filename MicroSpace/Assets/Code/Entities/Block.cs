using Attributes;
using ExtensionMethods;
using Maths;
using Miscellaneous;
using ScriptableObjects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Entities
{
    public abstract class Block : Entity
    {
        #region Fields

        [SerializeField]
        protected Colors _colors;

        [SerializeField]
        [ReadonlyInspector]
        private int _modelId;

        [SerializeField]
        [ReadonlyInspector]
        private int _shapeId;

        [SerializeField]
        [ReadonlyInspector]
        private bool _isMarkedForMining;

        protected Satellite _satellite;

        #endregion Fields

        #region Properties

        public Transform Transform => transform;

        public Transform Parent
        {
            get => transform.parent;
            set => transform.parent = value;
        }

        public bool IsMarkedForMining
        {
            get => _isMarkedForMining;
            set
            {
                if (_isMarkedForMining != value)
                    MiningMarkChanged.Invoke(value);
                _isMarkedForMining = value;
            }
        }

        public Square Square => new(transform.position, 0.48F, transform.eulerAngles.z);

        public Vector2Int FixedLocalPosition { get; private set; }

        public Vector2 LocalPosition => transform.localPosition;

        public float LocalRotation => transform.localEulerAngles.z;

        public BlockType BlockType { get; private set; }

        public int ModelId { get => _modelId; set => _modelId = value; }
        public int ShapeId { get => _shapeId; set => _shapeId = value; }

        public UnityEvent<bool> MiningMarkChanged { get; } = new();

        #endregion Properties

        #region Unity

        protected override void Awake()
        {
            base.Awake();
            SetFixedLocalPosition();
            SetBlockType();
        }

        protected virtual void Start()
        {
            SetSatellite();
            SetFixedLocalPosition();
        }

        private void SetSatellite()
        {
            _satellite = this.TryGetComponentUpInHierarchy(out Satellite satellite) ?
                            satellite : null;
        }

        #endregion Unity

        #region Private

        private void SetBlockType()
        {
            BlockType = this switch
            {
                Floor => BlockType.Floor,
                FloorDesignation => BlockType.FloorDesignation,
                WallDesignation => BlockType.WallDesignation,
                _ => BlockType.Wall
            };
        }

        private void SetFixedLocalPosition()
        {
            FixedLocalPosition = Vector2Int.RoundToInt(transform.localPosition);
        }

        #endregion Private
    }
}