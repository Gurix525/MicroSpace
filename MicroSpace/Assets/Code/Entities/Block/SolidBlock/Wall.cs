using System;
using System.Collections.Generic;
using ExtensionMethods;
using UnityEngine;

namespace Entities
{
    public class Wall : SolidBlock
    {
        #region Fields

        private SpriteMask _spriteMask;

        #endregion Fields

        #region Properties

        public bool IsIncludedInObstacle { get; set; }

        public static List<Wall> EnabledWalls { get; } = new();

        #endregion Properties

        #region Unity

        protected override void Start()
        {
            base.Start();
            _spriteMask = GetComponent<SpriteMask>();
            SetSpriteMaskOrder();
        }

        private void OnEnable()
        {
            AddSelfToList();
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

        private void SetSpriteMaskOrder()
        {
            _spriteMask.frontSortingOrder = _satellite.Id;
            _spriteMask.backSortingOrder = _satellite.Id - 1;
        }

        #endregion Private
    }
}