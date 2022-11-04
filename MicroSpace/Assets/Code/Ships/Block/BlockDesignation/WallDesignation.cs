using Data;
using UnityEngine;

namespace Ships
{
    public class WallDesignation : BlockDesignation
    {
        //private void FixedUpdate()
        //{
        //    if (IsCollidingWithAnotherBlock())
        //    {
        //        IsObstructed = true;
        //        _spriteRenderer.color = ColorBank.WallDesignationObstructed;
        //    }
        //    else
        //    {
        //        IsObstructed = false;
        //        _spriteRenderer.color = ColorBank.WallDesignationNormal;
        //    }
        //}

        private void OnTriggerStay2D(Collider2D collision)
        {
            _spriteRenderer.color = ColorBank.WallDesignationObstructed;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            _spriteRenderer.color = ColorBank.WallDesignationNormal;
        }
    }
}