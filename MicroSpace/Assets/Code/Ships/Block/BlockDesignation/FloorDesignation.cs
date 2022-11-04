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
        //        _spriteRenderer.color = _colors.FloorDesignationObstructed;
        //    }
        //    else
        //    {
        //        IsObstructed = false;
        //        _spriteRenderer.color = _colors.FloorDesignationNormal;
        //    }
        //}

        private void OnTriggerStay2D(Collider2D collision)
        {
            _spriteRenderer.color = _colors.FloorDesignationObstructed;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            _spriteRenderer.color = _colors.FloorDesignationNormal;
        }
    }
}