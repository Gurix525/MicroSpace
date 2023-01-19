using System;
using System.Collections.Generic;
using ExtensionMethods;
using ScriptableObjects;
using UnityEngine;

namespace Entities
{
    public class Wall : SolidBlock
    {
        #region Properties

        public bool IsIncludedInObstacle { get; set; }

        public static List<Wall> EnabledWalls { get; } = new();

        #endregion Properties

        #region Unity

        protected override void Start()
        {
            base.Start();
            TrySetSpriteMaskRange();
            AddSelfToSatelliteList();
            AddTile();
        }

        private void OnEnable()
        {
            AddSelfToEnabledList();
        }

        private void OnDisable()
        {
            RemoveSelfFromEnabledList();
        }

        private void OnDestroy()
        {
            RemoveSelfFromSatelliteList();
            RemoveTile();
        }

        #endregion Unity

        #region Private

        private void AddSelfToSatelliteList()
        {
            if (_satellite == null)
                return;
            _satellite.Blocks.Add(this);
            _satellite.SolidBlocks.Add(this);
            _satellite.Walls.Add(this);
        }

        private void AddTile()
        {
            _satellite.WallsTilemap.SetTile(
               (Vector3Int)FixedLocalPosition,
               BlockModel.GetModel(ModelId).Tile);
            _satellite.WallsTilemap.SetTileFlags(
                (Vector3Int)FixedLocalPosition,
                UnityEngine.Tilemaps.TileFlags.None);
        }

        private void RemoveTile()
        {
            _satellite.WallsTilemap.SetTile(
                (Vector3Int)FixedLocalPosition,
                null);
        }

        private void RemoveSelfFromSatelliteList()
        {
            if (_satellite == null)
                return;
            _satellite.Blocks.Remove(this);
            _satellite.SolidBlocks.Remove(this);
            _satellite.Walls.Remove(this);
        }

        private void AddSelfToEnabledList()
        {
            if (!EnabledWalls.Contains(this))
                EnabledWalls.Add(this);
        }

        private void RemoveSelfFromEnabledList()
        {
            if (EnabledWalls.Contains(this))
                EnabledWalls.Remove(this);
        }

        private void TrySetSpriteMaskRange()
        {
            if (!TryGetComponent(out SpriteMask mask))
                return;
            mask.isCustomRangeActive = true;
            mask.frontSortingOrder = _satellite.Id;
            mask.backSortingOrder = _satellite.Id - 1;
        }

        #endregion Private
    }
}