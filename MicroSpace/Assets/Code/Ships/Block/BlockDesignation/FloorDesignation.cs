using Data;
using UnityEngine;

namespace Ships
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