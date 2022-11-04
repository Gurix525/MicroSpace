using Assets.Code.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Ships
{
    [Serializable]
    public abstract class Block : MonoBehaviour, IBlock
    {
        #region Fields

        [SerializeField]
        private int _id;

        [SerializeField]
        private Vector2 _localPosition;

        [SerializeField]
        private bool _isMarkedForMining;

        #endregion Fields

        #region Properties

        public int Id { get => _id; set => _id = value; }

        public Transform Transform => transform;

        public Transform Parent
        {
            get => transform.parent;
            set => transform.parent = value;
        }

        public bool IsMarkedForMining { get => _isMarkedForMining; set => _isMarkedForMining = value; }

        public Square Square => new(transform.position, 0.4999F, transform.eulerAngles);

        public Vector2 LocalPosition { get => _localPosition; set => _localPosition = value; }

        #endregion Properties

        public void UpdateBlock()
        {
            LocalPosition = transform.localPosition;
        }

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
    }
}