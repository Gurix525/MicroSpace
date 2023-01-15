using UnityEngine;
using ExtensionMethods;
using ScriptableObjects;
using System;
using UnityEngine.Events;

namespace Entities
{
    public abstract class BlockDesignation : Block
    {
        #region Fields

        [SerializeField]
        private GameObject _finishedBlockPrefab;

        private bool _isObstructed = true;

        #endregion Fields

        #region Properties

        public bool IsObstructed
        {
            get => _isObstructed;
            protected set
            {
                if (_isObstructed != value)
                    ObstructedStateChanged.Invoke(value);
                _isObstructed = value;
            }
        }

        public UnityEvent<bool> ObstructedStateChanged = new();

        #endregion Properties

        #region Public

        public void BuildBlock()
        {
            if (this is not WallDesignation && this is not FloorDesignation)
                throw new InvalidOperationException("BuildBlock jest dostępne" +
                    "tylko dla WallDesignation i FloorDesignation. Możliwe, że" +
                    "próbujesz użyć BuildBlock dla desygnatu, który nie jest " +
                    "jednym z podanych");
            GameObject newBlock = Instantiate(
                _finishedBlockPrefab,
                transform.position,
                transform.rotation,
                transform.parent);
            Block newBlockComponent = newBlock.GetComponent<Block>();
            SetBlockShape(newBlockComponent, ShapeId);
            SetBlockModel(newBlockComponent, ModelId);
            Satellite satellite = this.GetComponentUpInHierarchy<Satellite>();
            transform.parent = null;
            Destroy(gameObject);
        }

        #endregion Public

        #region Private

        private void SetBlockShape(Block block, int shapeId)
        {
            if (block is Wall)
            {
                Shape shape = Shape.GetShape(shapeId);
                block.ShapeId = shapeId;
                if (block.TryGetComponent(out SpriteMask mask))
                {
                    mask.sprite = shape.Sprite;
                }
                if (block.TryGetComponent(out PolygonCollider2D collider))
                {
                    PolygonCollider2D shapeCollider = shape.Prefab.GetComponent<PolygonCollider2D>();
                    for (int i = 0; i < collider.pathCount; i++)
                        collider.SetPath(i, shapeCollider.GetPath(i));
                }
            }
            else
            {
                ShapeId = 0;
                if (block.TryGetComponent(out SpriteMask mask))
                {
                    mask.sprite = Shape.GetShape(0).Sprite;
                }
            }
        }

        private void SetBlockModel(Block block, int modelId)
        {
            block.ModelId = modelId;
        }

        #endregion Private
    }
}