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
                Collider2D collider = Physics2D.OverlapBox(
                    transform.position,
                    new Vector2(0.49F, 0.49F),
                    transform.rotation.z,
                    TemporalBlockType switch
                    {
                        BlockType.WallDesignation => LayerMask.GetMask("Walls", "WallDesignations"),
                        _ => LayerMask.GetMask("Floors", "FloorDesignations")
                    });
                if (collider != null)
                {
                    IsObstructed = true;
                    _spriteRenderer.color = _colors.TemporalDesignationObstructed;
                    return;
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