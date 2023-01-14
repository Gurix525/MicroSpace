using UnityEngine;

namespace Entities
{
    public class TemporalDesignation : BlockDesignation
    {
        #region Fields

        protected SpriteRenderer _spriteRenderer;

        #endregion Fields

        #region Properties

        public BlockType TemporalBlockType { get; set; }

        #endregion Properties

        #region Unity

        protected override void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sortingOrder = Id;
            TrySetSpriteMaskRange();
        }

        protected virtual void Update()
        {
            if (transform.parent != null)
            {
                if (IsCollidingWithAnotherBlocks(out Block[] blocks))
                {
                    foreach (var block in blocks)
                    {
                        if (block.BlockType == TemporalBlockType ||
                        block.BlockType == TemporalBlockType - 2)
                        {
                            IsObstructed = true;
                            _spriteRenderer.color = _colors.TemporalDesignationObstructed;
                            return;
                        }
                    }
                }
                IsObstructed = false;
                _spriteRenderer.color = _colors.TemporalDesignationNormal;
            }
        }

        #endregion Unity

        #region Protected

        private void TrySetSpriteMaskRange()
        {
            if (TryGetComponent(out SpriteMask mask))
            {
                mask.isCustomRangeActive = true;
                mask.frontSortingOrder = Id;
                mask.backSortingOrder = Id - 1;
            }
        }

        #endregion Protected
    }
}