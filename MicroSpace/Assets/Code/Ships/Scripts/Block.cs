using Assets.Code.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Ships
{
    public abstract class Block : MonoBehaviour, IBlock
    {
        public Transform Transform => transform;

        public Transform Parent
        {
            get => transform.parent;
            set => transform.parent = value;
        }

        public bool IsMarkedForMining { get; set; }

        protected bool IsCollidingWithAnotherBlock()
        {
            return IsCollidingWithAnotherBlock(out _);
        }

        protected bool IsCollidingWithAnotherBlock(out Block collidingBlock)
        {
            var blocks = FindObjectsOfType<Block>()
                .Where(x => Vector2.Distance(transform.position, x.transform.position) < 1.42F)
                .Where(x => x != this)
                .Where(x => x is not TemporalDesignation);
            foreach (var block in blocks)
            {
                if (Square.IsIntersecting(block.Square))
                {
                    collidingBlock = block;
                    return true;
                }
            }
            collidingBlock = null;
            return false;
        }

        public Square Square => new(transform.position, 0.4999F, transform.eulerAngles);
    }
}