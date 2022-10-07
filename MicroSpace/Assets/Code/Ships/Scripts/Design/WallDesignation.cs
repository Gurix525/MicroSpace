using Assets.Code.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Ships
{
    public class WallDesignation : BlockDesignation
    {
        private void FixedUpdate()
        {
            if (IsCollidingWithAnotherBlock())
            {
                IsObstructed = true;
                _spriteRenderer.color = ColorBank.WallDesignationObstructed;
            }
            else
            {
                IsObstructed = false;
                _spriteRenderer.color = ColorBank.WallDesignationNormal;
            }
        }
    }
}