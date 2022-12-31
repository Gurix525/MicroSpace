using System.Collections.Generic;
using System.Linq;
using Miscellaneous;
using ScriptableObjects;
using UnityEngine;

namespace Tasks
{
    public class TaskProvider : MonoBehaviour
    {
        [SerializeField]
        private ItemModel[] _itemModels;

        [SerializeField]
        private float[] _itemAmounts;

        [SerializeField]
        private ToolType _toolType;

        public Task GetTask()
        {
            return new(
                _itemModels.Select(model => model.Id).ToArray(),
                _itemAmounts,
                (int)_toolType,
                0,
                0,
                0);
        }

        private void OnValidate()
        {
            if (_itemModels.Length != _itemAmounts.Length)
                Debug.LogError("ItemModels and ItemAmounts muszą posiadać tyle" +
                    "samo elementów");
        }
    }
}