using UnityEngine;

namespace Entities
{
    public class TemporalDesignation : BlockDesignation
    {
        public BlockType TemporalBlockType { get; set; }

        private void Update()
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

        protected override void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}