using Assets.Code.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Ships
{
    public class CancelDesignation : BlockDesignation
    {
        private void Update()
        {
            if (IsCollidingWithAnotherBlock(out Block collidingBlock) ?
                collidingBlock is BlockDesignation : false)
            {
                IsObstructed = true;
                _spriteRenderer.color = ColorBank.CancelDesignationActive;
            }
            else
            {
                IsObstructed = false;
                _spriteRenderer.color = ColorBank.CancelDesignationInactive;
            }
        }
    }
}