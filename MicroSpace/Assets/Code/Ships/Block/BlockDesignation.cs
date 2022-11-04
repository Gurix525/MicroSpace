using UnityEngine;

namespace Ships
{
    public class BlockDesignation : Block
    {
        protected SpriteRenderer _spriteRenderer;

        public bool IsObstructed { get; protected set; }

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}