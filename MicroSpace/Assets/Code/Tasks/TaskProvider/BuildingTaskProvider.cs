using System;
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

        private void Execute()
        {
            _blockDesignation.BuildBlock();
        }
    }
}