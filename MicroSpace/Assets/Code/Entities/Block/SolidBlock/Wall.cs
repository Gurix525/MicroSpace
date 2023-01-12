using UnityEngine;

namespace Entities
{
    public class Wall : SolidBlock
    {
        private SpriteMask _spriteMask;
        private SpriteRenderer _renderer;
        public bool IsIncludedInObstacle { get; set; }

        protected override void Awake()
        {
            base.Awake();
            _spriteMask = GetComponent<SpriteMask>();
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void FixedUpdate()
        {
            _spriteMask.sortingLayerName = _renderer.sortingLayerName;
        }
    }
}