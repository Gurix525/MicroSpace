using System;
using UnityEngine;

namespace Ships
{
    [Serializable]
    public class SerializableBlock
    {
        [SerializeField]
        private int _id;

        [SerializeField]
        private BlockType _blockType;

        [SerializeField]
        private Vector2 _localPosition;

        [SerializeField]
        private float _localRotation;

        [SerializeField]
        private bool _isMarkedForMining;

        public int Id => _id;
        public bool IsMarkedForMining => _isMarkedForMining;
        public Vector2 LocalPosition => _localPosition;
        public float LocalRotation => _localRotation;
        public BlockType BlockType => _blockType;

        public SerializableBlock(Block block)
        {
            _id = block.Id;
            _localPosition = block.LocalPosition;
            _localRotation = block.LocalRotation;
            _isMarkedForMining = block.IsMarkedForMining;
            _blockType = block.BlockType;
        }
    }
}