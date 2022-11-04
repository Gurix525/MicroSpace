using UnityEngine;

namespace Ships
{
    public class TemporalDesignation : BlockDesignation
    {
        private void Update()
        {
            if (transform.parent != null)
            {
                if (IsCollidingWithAnotherBlock())
                {
                    IsObstructed = true;
                    _spriteRenderer.color = _colors.TemporalDesignationObstructed;
                }
                else
                {
                    IsObstructed = false;
                    _spriteRenderer.color = _colors.TemporalDesignationNormal;
                }
            }
        }

        protected override void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}