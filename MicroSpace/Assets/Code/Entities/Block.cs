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

        public Vector2 LocalPosition => transform.localPosition;

        public float LocalRotation => transform.localEulerAngles.z;

        public BlockType BlockType => this switch
        {
            Floor => BlockType.Floor,
            FloorDesignation => BlockType.FloorDesignation,
            WallDesignation => BlockType.WallDesignation,
            _ => BlockType.Wall
        };

        public int ModelId { get => _modelId; set => _modelId = value; }
        public int ShapeId { get => _shapeId; set => _shapeId = value; }

        public UnityEvent<bool> MiningMarkChanged { get; } = new();

        #endregion Properties

        #region Unity

        protected virtual void Start()
        {
            _satellite = this.TryGetComponentUpInHierarchy(out Satellite satellite) ?
                satellite : null;
        }

        #endregion Unity

        #region Protected

        protected bool IsCollidingWithAnotherBlock()
        {
            return IsCollidingWithAnotherBlock(out _);
        }

        protected bool IsCollidingWithAnotherBlock(out Block collidingBlock)
        {
            var blocks = FindObjectsOfType<Block>()
                .Where(x =>
                Vector2.Distance(transform.position, x.transform.position) < 1.42F &&
                x != this &&
                x is not TemporalDesignation);
            foreach (var block in blocks)
            {
                if (Square.IsIntersecting(block.Square))
                {
                    collidingBlock = block;
                    return true;
                }
            }
            collidingBlock = null;
            return false;
        }

        protected bool IsCollidingWithAnotherBlocks(out Block[] collidingBlocks)
        {
            List<Block> newCollidingBlocks = new();
            var blocks = FindObjectsOfType<Block>()
                .Where(x =>
                Vector2.Distance(transform.position, x.transform.position) < 1.42F &&
                x != this &&
                x is not TemporalDesignation);
            foreach (var block in blocks)
            {
                if (Square.IsIntersecting(block.Square))
                {
                    newCollidingBlocks.Add(block);
                }
            }
            collidingBlocks = newCollidingBlocks.ToArray();
            return collidingBlocks.Length > 0;
        }

        #endregion Protected
    }
}