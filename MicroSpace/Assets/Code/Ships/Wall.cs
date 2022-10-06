using Assets.Code.Data.PartDataImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Ships
{
    /// <summary>
    /// Ma swój odpowiednik w Data.WallData.
    /// </summary>
    public class Wall : MonoBehaviour, IBlock
    {
        [SerializeField]
        private WallData _wallData;

        public Transform Transform => transform;

        public Transform Parent
        {
            get => transform.parent;
            set => transform.parent = value;
        }

        public WallData WallData { get => _wallData; set => _wallData = value; }
    }
}