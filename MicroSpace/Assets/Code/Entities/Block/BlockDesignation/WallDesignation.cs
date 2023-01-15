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
            SetSpriteMaskRange();
            AddSelfToSatelliteList();
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Block block))
            {
                if (block.BlockType == BlockType.Wall)
                {
                    _satellite.WallDesignationsTilemap.SetColor(
                        Vector3Int.RoundToInt(LocalPosition),
                        _colors.WallDesignationObstructed);
                    IsObstructed = true;
                }
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
        }

        #endregion Unity

        #region Private

        private void SetSpriteMaskRange()
        {
            SpriteMask mask = GetComponent<SpriteMask>();
            if (this.TryGetComponentUpInHierarchy(out Satellite satellite))
            {
                mask.isCustomRangeActive = true;
                mask.frontSortingOrder = satellite.Id;
                mask.backSortingOrder = satellite.Id - 1;
            }
        }

        private void AddSelfToSatelliteList()
        {
            if (_satellite == null)
                return;
            _satellite.Blocks.Add(this);
            _satellite.WallDesignations.Add(this);
            _satellite.WallDesignationsTilemap.SetTile(
                Vector3Int.RoundToInt(LocalPosition),
                BlockModel.GetModel(ModelId).Tile);
            _satellite.WallDesignationsTilemap.SetTileFlags(
                Vector3Int.RoundToInt(LocalPosition),
                UnityEngine.Tilemaps.TileFlags.None);
        }

        private void RemoveSelfFromSatelliteList()
        {
            if (_satellite == null)
                return;
            _satellite.Blocks.Remove(this);
            _satellite.WallDesignations.Remove(this);
            _satellite.WallDesignationsTilemap.SetTile(
                Vector3Int.RoundToInt(LocalPosition),
                null);
        }

        #endregion Private
    }
}