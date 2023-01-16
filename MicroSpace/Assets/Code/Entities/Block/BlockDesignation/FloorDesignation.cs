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
            AddTile();
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Floors"))
            {
                _satellite.FloorDesignationsTilemap.SetColor(
                    Vector3Int.RoundToInt(FixedLocalPosition),
                    _colors.FloorDesignationObstructed);
                IsObstructed = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            _satellite.FloorDesignationsTilemap.SetColor(
                        Vector3Int.RoundToInt(FixedLocalPosition),
                        _colors.FloorDesignationNormal);
            IsObstructed = false;
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
            _satellite.FloorDesignations.Add(this);
        }

        private void AddTile()
        {
            _satellite.FloorDesignationsTilemap.SetTile(
                FixedLocalPosition,
                BlockModel.GetModel(ModelId).Tile);
            _satellite.FloorDesignationsTilemap.SetTileFlags(
                FixedLocalPosition,
                UnityEngine.Tilemaps.TileFlags.None);
        }

        private void RemoveSelfFromSatelliteList()
        {
            if (_satellite == null)
                return;
            _satellite.Blocks.Remove(this);
            _satellite.FloorDesignations.Remove(this);
        }

        private void RemoveTile()
        {
            _satellite.FloorDesignationsTilemap.SetTile(
                FixedLocalPosition,
                null);
        }

        #endregion Private
    }
}