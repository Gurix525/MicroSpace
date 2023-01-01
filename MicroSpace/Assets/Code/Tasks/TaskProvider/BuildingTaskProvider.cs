using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Miscellaneous;
using ScriptableObjects;
using UnityEngine;

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
                    blockModel.ItemModels
                    .Select(model => model.Id)
                    .ToArray(),
                blockModel.ItemAmounts,
                (int)_toolType,
                0,
                0,
                0);
        }

        private void Start()
        {
            TaskSource.AddTask(GetTask());
        }
    }
}