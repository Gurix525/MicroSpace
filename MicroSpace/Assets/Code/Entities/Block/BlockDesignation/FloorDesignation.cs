using System;
using ScriptableObjects;
using UnityEngine;

namespace Entities
{
    public class FloorDesignation : BlockDesignation
    {
        #region Unity

        protected override void Start()
        {
            base.Start();
            IsObstructed = false;
            AddSelfToSatelliteList();
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Block block))
            {
                if (block.BlockType == BlockType.Floor)
                {
                    _satellite.FloorDesignationsTilemap.SetColor(
                        Vector3Int.RoundToInt(LocalPosition),
                        _colors.FloorDesignationObstructed);
                    IsObstructed = true;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            _satellite.FloorDesignationsTilemap.SetColor(
                        Vector3Int.RoundToInt(LocalPosition),
                        _colors.FloorDesignationNormal);
            IsObstructed = false;
        }

        private void OnDestroy()
        {
            RemoveSelfFromSatelliteList();
        }

        #endregion Unity

        #region Private

        private void AddSelfToSatelliteList()
        {
            if (_satellite == null)
                return;
            _satellite.Blocks.Add(this);
            _satellite.FloorDesignations.Add(this);
            _satellite.FloorDesignationsTilemap.SetTile(
                Vector3Int.RoundToInt(LocalPosition),
                BlockModel.GetModel(ModelId).Tile);
            _satellite.FloorDesignationsTilemap.SetTileFlags(
                Vector3Int.RoundToInt(LocalPosition),
                UnityEngine.Tilemaps.TileFlags.None);
        }

        private void RemoveSelfFromSatelliteList()
        {
            if (_satellite == null)
                return;
            _satellite.Blocks.Remove(this);
            _satellite.FloorDesignations.Remove(this);
            _satellite.WallsTilemap.SetTile(
                Vector3Int.RoundToInt(LocalPosition),
                null);
        }

        #endregion Private
    }
}