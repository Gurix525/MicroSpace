using UnityEngine;

namespace Ships
{
    public interface IBlock
    {
        public Transform Transform { get; }

        public Transform Parent { get; set; }
    }
}