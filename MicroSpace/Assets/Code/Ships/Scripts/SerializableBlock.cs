using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Ships
{
    [Serializable]
    public class SerializableBlock
    {
        [SerializeField]
        private int _id;

        [SerializeField]
        private Vector2 _localPosition;

        [SerializeField]
        private bool _isMarkedForMining;

        public int Id => _id;
        public bool IsMarkedForMining => _isMarkedForMining;
        public Vector2 LocalPosition => _localPosition;

        public SerializableBlock(Block block)
        {
            _id = block.Id;
            _localPosition = block.LocalPosition;
            _isMarkedForMining = block.IsMarkedForMining;
        }
    }
}