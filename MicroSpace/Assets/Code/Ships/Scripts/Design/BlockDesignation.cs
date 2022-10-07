using Assets.Code.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Ships
{
    public class BlockDesignation : Block
    {
        protected SpriteRenderer _spriteRenderer;

        public bool IsObstructed { get; protected set; }

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}