using System;
using ExtensionMethods;
using ScriptableObjects;
using UnityEngine;

namespace Entities
{
    public class WallDesignation : BlockDesignation
    {
        #region Unity

        protected override void Start()
        {
            base.Start();
            IsObstructed = false;
            TrySetSpriteMaskRange();
            AddSelfToSatelliteList();
            AddTile();
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Walls"))
            {
                _satellite.WallDesignationsTilemap.SetColor(
                    Vector3Int.RoundToInt(LocalPosition),
                    _colors.WallDesignationObstructed);
                IsObstructed = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            _satellite.WallDesignationsTilemap.SetColor(
                Vector3Int.RoundToInt(LocalPosition),
                _colors.WallDesignationNormal);
            IsObstructed = false;
        }

        private void OnDestroy()
        {
            RemoveSelfFromSatelliteList();
            RemoveTile();
        }

        #endregion Unity

        #region Private

        private void TrySetSpriteMaskRange()
        {
            if (!TryGetComponent(out SpriteMask mask))
                return;
            mask.isCustomRangeActive = true;
            mask.frontSortingOrder = _satellite.Id;
            mask.backSortingOrder = _satellite.Id - 1;
        }

        private void AddSelfToSatelliteList()
        {
            if (_satellite == null)
                return;
            _satellite.Blocks.Add(this);
            _satellite.WallDesignations.Add(this);
        }

        private void AddTile()
        {
            _satellite.WallDesignationsTilemap.SetTile(
                FixedLocalPosition,
                BlockModel.GetModel(ModelId).Tile);
            _satellite.WallDesignationsTilemap.SetTileFlags(
                FixedLocalPosition,
                UnityEngine.Tilemaps.TileFlags.None);
        }

        private void RemoveSelfFromSatelliteList()
        {
            if (_satellite == null)
                return;
            _satellite.Blocks.Remove(this);
            _satellite.WallDesignations.Remove(this);
        }

        private void RemoveTile()
        {
            _satellite.WallDesignationsTilemap.SetTile(
                FixedLocalPosition,
                null);
        }

        #endregion Private
    }
}