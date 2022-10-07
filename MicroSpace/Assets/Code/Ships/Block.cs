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

        protected bool IsCollidingWithAnotherBlock()
        {
            var blocks = FindObjectsOfType<Block>()
                .Where(x => Vector2.Distance(transform.position, x.transform.position) < 2)
                .Where(x => x != this);
            foreach (var block in blocks)
            {
                if (Square.IsIntersecting(block.Square))
                    return true;
            }
            return false;
        }

        public Square Square => new(transform.position, 0.49F, transform.eulerAngles);
    }
}