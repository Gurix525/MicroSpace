using Attributes;
using Maths;
using ScriptableObjects;
using System;
using System.Linq;
using UnityEngine;

namespace Ships
{
    [Serializable]
    public abstract class Block : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        protected IdManagerScriptableObject _idManager;

        [SerializeField]
        protected ColorsScriptableObject _colors;

        [SerializeField]
        [ReadonlyInspector]
        private int _id;

        [SerializeField]
        [ReadonlyInspector]
        private int _modelId;

        [SerializeField]
        [ReadonlyInspector]
        private int _shapeId;

        [SerializeField]
        [ReadonlyInspector]
        private Vector2 _localPosition;

        [SerializeField]
        [ReadonlyInspector]
        private float _localRotation;

        [SerializeField]
        [ReadonlyInspector]
        private bool _isMarkedForMining;

        #endregion Fields

        #region Properties

        public int Id { get => _id; set => _id = value; }

        public Transform Transform => transform;

        public Transform Parent
        {
            get => transform.parent;
            set => transform.parent = value;
        }

        public bool IsMarkedForMining { get => _isMarkedForMining; set => _isMarkedForMining = value; }

        public Square Square => new(transform.position, 0.48F, transform.eulerAngles);

        public Vector2 LocalPosition { get => _localPosition; set => _localPosition = value; }

        public float LocalRotation { get => _localRotation; set => _localRotation = value; }

        public BlockType BlockType => this switch
        {
            Floor => BlockType.Floor,
            FloorDesignation => BlockType.FloorDesignation,
            WallDesignation => BlockType.WallDesignation,
            _ => BlockType.Wall
        };

        public int ModelId { get => _modelId; set => _modelId = value; }
        public int ShapeId { get => _shapeId; set => _shapeId = value; }

        #endregion Properties

        #region Public

        public void UpdateBlock()
        {
            LocalPosition = transform.localPosition;
            LocalRotation = transform.localEulerAngles.z;
        }

        #endregion Public

        #region Protected

        protected bool IsCollidingWithAnotherBlock()
        {
            return IsCollidingWithAnotherBlock(out _);
        }

        protected void TrySetSpriteMaskRange()
        {
            if (TryGetComponent(out SpriteMask mask))
            {
                mask.isCustomRangeActive = true;
                mask.backSortingOrder = Id - 1;
                mask.frontSortingOrder = Id;
            }
        }

        protected bool IsCollidingWithAnotherBlock(out Block collidingBlock)
        {
            var blocks = FindObjectsOfType<Block>()
                .Where(x => Vector2.Distance(transform.position, x.transform.position) < 1.42F)
                .Where(x => x != this)
                .Where(x => x is not TemporalDesignation);
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

        protected virtual void SetId()
        {
            if (Id == 0)
                Id = _idManager.NextId;
        }

        #endregion Protected

        #region Unity

        protected virtual void Awake()
        {
            SetId();
        }

        protected virtual void Start()
        {
            SetSpriteOrderInLater();
            TrySetSpriteMaskRange();
        }

        protected void SetSpriteOrderInLater()
        {
            GetComponent<SpriteRenderer>().sortingOrder = Id;
        }

        #endregion Unity
    }
}