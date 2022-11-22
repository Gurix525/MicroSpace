using System;
using UnityEngine;

namespace Entities
{
    [Serializable]
    public class SerializableBlock
    {
        // Pola nie mogą być readonly bo serializacja nie zadziała

        [SerializeField]
        private int _id;

        [SerializeField]
        private BlockType _blockType;

        [SerializeField]
        private int _modelId;

        [SerializeField]
        private int _shapeId;

        [SerializeField]
        private Vector2 _localPosition;

        [SerializeField]
        private float _localRotation;

        [SerializeField]
        private bool _isMarkedForMining;

        public int Id => _id;
        public int ModelId => _modelId;
        public int ShapeId => _shapeId;
        public bool IsMarkedForMining => _isMarkedForMining;
        public Vector2 LocalPosition => _localPosition;
        public float LocalRotation => _localRotation;
        public BlockType BlockType => _blockType;

        public SerializableBlock(Block block)
        {
            _id = block.Id;
            _modelId = block.ModelId;
            _shapeId = block.ShapeId;
            _localPosition = block.LocalPosition;
            _localRotation = block.LocalRotation;
            _isMarkedForMining = block.IsMarkedForMining;
            _blockType = block.BlockType;
        }
    }
}