﻿using Data;

namespace Ships
{
    public class TemporalDesignation : BlockDesignation
    {
        private void Update()
        {
            if (transform.parent != null)
            {
                if (IsCollidingWithAnotherBlock())
                {
                    IsObstructed = true;
                    _spriteRenderer.color = ColorBank.TemporalDesignationObstructed;
                }
                else
                {
                    IsObstructed = false;
                    _spriteRenderer.color = ColorBank.TemporalDesignationNormal;
                }
            }
        }
    }
}