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
        private bool _isMarkedForMining;

        public int Id => _id;
        public bool IsMarkedForMining => _isMarkedForMining;
        public Vector2 LocalPosition => _localPosition;
        public BlockType BlockType => _blockType;

        public SerializableBlock(Block block)
        {
            _id = block.Id;
            _localPosition = block.LocalPosition;
            _isMarkedForMining = block.IsMarkedForMining;
            _blockType = block switch
            {
                Floor => BlockType.Floor,
                FloorDesignation => BlockType.FloorDesignation,
                WallDesignation => BlockType.WallDesignation,
                _ => BlockType.Wall
            };
        }
    }
}