using UnityEngine;

namespace Ships
{
    public abstract class BlockDesignation : Block
    {
        protected SpriteRenderer _spriteRenderer;

        public bool IsObstructed { get; protected set; } = true;

        protected override void Awake()
        {
            base.Awake();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}