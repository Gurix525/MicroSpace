using Entities;
using ScriptableObjects;
using System.Linq;

namespace Tasks
{
    public class BuildingTaskProvider : TaskProvider
    {
        private BlockDesignation _blockDesignation;

        private BlockDesignation BlockDesignation =>
            _blockDesignation ??= GetComponent<BlockDesignation>();

        protected override Task GetTask()
        {
            BlockModel blockModel = BlockModel.GetModel(BlockDesignation.ModelId);
            return new(
                transform,
                BlockDesignation.Id,
                blockModel.ItemModels
                    .Select(model => model.Id)
                    .ToArray(),
                blockModel.ItemAmounts,
                (int)_toolType,
                0,
                0,
                0,
                1,
                Execute);
        }

        private void Start()
        {
            TaskSource.AddTask(GetTask());
        }

        protected override void Execute()
        {
            _blockDesignation.BuildBlock();
        }
    }
}