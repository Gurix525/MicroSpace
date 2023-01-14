using System;
using ExtensionMethods;
using UnityEngine;

namespace Entities
{
    public class WallDesignation : BlockDesignation
    {
        protected override void Start()
        {
            base.Start();
            IsObstructed = false;
            SetSpriteMaskRange();
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

        protected void SetSpriteMaskRange()
        {
            SpriteMask mask = GetComponent<SpriteMask>();
            if (this.TryGetComponentUpInHierarchy(out Satellite satellite))
            {
                mask.isCustomRangeActive = true;
                mask.frontSortingOrder = satellite.Id;
                mask.backSortingOrder = satellite.Id - 1;
            }
        }
    }
}