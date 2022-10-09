using Assets.Code.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Ships
{
    public class TemporalDesignation : BlockDesignation
    {
        private void Update()
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