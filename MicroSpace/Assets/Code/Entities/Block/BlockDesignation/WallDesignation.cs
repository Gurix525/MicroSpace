using UnityEngine;

namespace Entities
{
    public class WallDesignation : BlockDesignation
    {
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Block block))
            {
                if (block.BlockType == BlockType.Wall)
                {
                    _spriteRenderer.color = _colors.WallDesignationObstructed;
                    IsObstructed = true;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            _spriteRenderer.color = _colors.WallDesignationNormal;
            IsObstructed = false;
        }
    }
}