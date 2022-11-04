using Assets.Code.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Ships
{
    public class FloorDesignation : BlockDesignation
    {
        //private void FixedUpdate()
        //{
        //    if (IsCollidingWithAnotherBlock())
        //    {
        //        IsObstructed = true;
        //        _spriteRenderer.color = ColorBank.FloorDesignationObstructed;
        //    }
        //    else
        //    {
        //        IsObstructed = false;
        //        _spriteRenderer.color = ColorBank.FloorDesignationNormal;
        //    }
        //}

        private void OnTriggerStay2D(Collider2D collision)
        {
            _spriteRenderer.color = ColorBank.FloorDesignationObstructed;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            _spriteRenderer.color = ColorBank.FloorDesignationNormal;
        }
    }
}