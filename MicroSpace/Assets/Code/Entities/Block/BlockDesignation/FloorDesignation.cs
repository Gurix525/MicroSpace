using UnityEngine;

namespace Entities
{
    public class FloorDesignation : BlockDesignation
    {
        protected override void Start()
        {
            base.Start();
            IsObstructed = false;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Block block))
            {
                if (block.BlockType == BlockType.Floor)
                {
                    _spriteRenderer.color = _colors.FloorDesignationObstructed;
                    IsObstructed = true;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            _spriteRenderer.color = _colors.FloorDesignationNormal;
            IsObstructed = false;
        }
    }
}