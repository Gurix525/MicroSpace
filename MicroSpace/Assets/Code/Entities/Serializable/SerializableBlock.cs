using System;
using UnityEngine;

namespace Entities
{
    [Serializable]
    public class SerializableBlock
    {
        [field: SerializeField]
        public int Id { get; private set; }

        [field: SerializeField]
        public int ModelId { get; private set; }

        [field: SerializeField]
        public int ShapeId { get; private set; }

        [field: SerializeField]
        public bool IsMarkedForMining { get; private set; }

        [field: SerializeField]
        public Vector2 LocalPosition { get; private set; }

        [field: SerializeField]
        public float LocalRotation { get; private set; }

        [field: SerializeField]
        public BlockType BlockType { get; private set; }

        public SerializableBlock(Block block)
        {
            Id = block.Id;
            ModelId = block.ModelId;
            ShapeId = block.ShapeId;
            LocalPosition = block.LocalPosition;
            LocalRotation = block.LocalRotation;
            IsMarkedForMining = block.IsMarkedForMining;
            BlockType = block.BlockType;
        }
    }
}