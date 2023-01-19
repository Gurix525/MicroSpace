using System;
using Miscellaneous;
using ScriptableObjects;
using UnityEngine;

namespace Entities
{
    public class FloorDesignation : BlockDesignation
    {
        #region Fields

        private int _timer = 0;
        private bool _isColliding = false;

        #endregion Fields

        #region Unity

        protected override void Start()
        {
            base.Start();
            IsObstructed = false;
            AddSelfToSatelliteList();
            AddTile();
        }

        private void FixedUpdate()
        {
            _timer++;
            if (_timer >= 5)
            {
                _timer = 0;
                if (_isColliding)
                    SetObstructed();
                else
                    SetUnobstructed();
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.layer == References.FloorsLayer)
            {
                _isColliding = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == References.FloorsLayer)
            {
                _isColliding = false;
            }
        }

        private void OnDestroy()
        {
            RemoveSelfFromSatelliteList();
            RemoveTile();
        }

        #endregion Unity

        #region Private

        private void SetObstructed()
        {
            _satellite.FloorDesignationsTilemap.SetColor(
                (Vector3Int)FixedLocalPosition,
                _colors.FloorDesignationObstructed);
            IsObstructed = true;
        }

        private void SetUnobstructed()
        {
            _satellite.FloorDesignationsTilemap.SetColor(
                (Vector3Int)FixedLocalPosition,
                _colors.FloorDesignationNormal);
            IsObstructed = false;
        }

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
                (Vector3Int)FixedLocalPosition,
                BlockModel.GetModel(ModelId).Tile);
            _satellite.FloorDesignationsTilemap.SetTileFlags(
                (Vector3Int)FixedLocalPosition,
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
                (Vector3Int)FixedLocalPosition,
                null);
        }

        #endregion Private
    }
}