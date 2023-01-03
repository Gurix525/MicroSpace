using Entities;
using ScriptableObjects;
using UnityEngine;

namespace Tasks
{
    public class MiningTaskProvider : TaskProvider
    {
        private Block _block;
        private Task _task;

        private Block Block =>
            _block ??= GetComponent<Block>();

        private Task Task =>
            _task ??= GetTask();

        protected override Task GetTask()
        {
            BlockModel blockModel = BlockModel.GetModel(Block.ModelId);
            return new(
                transform,
                Block.Id,
                new int[0],
                new float[0],
                (int)_toolType,
                0,
                0,
                0,
                1,
                Execute);
        }

        private void Start()
        {
            Block.MiningMarkChanged.AddListener(OnMiningMarkChanged);
        }

        private void OnMiningMarkChanged(bool state)
        {
            if (state)
            {
                TaskSource.AddTask(Task);
            }
            else
                TaskSource.RemoveTask(Task);
        }

        protected override void Execute()
        {
            Destroy(gameObject);
        }
    }
}