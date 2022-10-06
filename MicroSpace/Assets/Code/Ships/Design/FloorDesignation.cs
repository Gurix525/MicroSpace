using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Ships
{
    public class FloorDesignation : MonoBehaviour, IBlock
    {
        public Transform Transform => transform;

        public Transform Parent
        {
            get => transform.parent;
            set => transform.parent = value;
        }
    }
}