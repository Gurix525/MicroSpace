using UnityEngine;
using ExtensionMethods;
using ScriptableObjects;

namespace Entities
{
    public abstract class BlockDesignation : Block
    {
        [SerializeField]
        private GameObject _finishedBlockPrefab;

        protected SpriteRenderer _spriteRenderer;

        public bool IsObstructed { get; protected set; } = true;

        protected override void Awake()
        {
            base.Awake();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void BuildBlock()
        {
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
            satellite.UpdateSatellite();
            Destroy(gameObject);
        }

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
            if (block.TryGetComponent(out SpriteRenderer renderer))
            {
                renderer.sprite = BlockModel.GetModel(modelId).Sprite;
            }
        }
    }
}