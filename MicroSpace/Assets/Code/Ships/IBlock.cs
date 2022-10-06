using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Ships
{
    public interface IBlock
    {
        public Transform Transform { get; }

        public Transform Parent { get; set; }
    }
}