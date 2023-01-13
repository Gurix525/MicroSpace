using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class Wall : SolidBlock
    {
        #region Fields

        private SpriteMask _spriteMask;
        private SpriteRenderer _renderer;

        #endregion Fields

        #region Properties

        public bool IsIncludedInObstacle { get; set; }

        public static List<Wall> EnabledWalls { get; } = new();

        #endregion Properties

        #region Unity

        protected override void Awake()
        {
            base.Awake();
            _spriteMask = GetComponent<SpriteMask>();
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            AddSelfToList();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            _spriteMask.sortingLayerName = _renderer.sortingLayerName;
        }

        private void OnDisable()
        {
            RemoveSelfFromList();
        }

        #endregion Unity

        #region Private

        private void AddSelfToList()
        {
            if (!EnabledWalls.Contains(this))
                EnabledWalls.Add(this);
        }

        private void RemoveSelfFromList()
        {
            if (EnabledWalls.Contains(this))
                EnabledWalls.Remove(this);
        }

        #endregion Private
    }
}