using System.Collections.Generic;
using System.Linq;
using Miscellaneous;
using ScriptableObjects;
using UnityEngine;

namespace Tasks
{
    [RequireComponent(typeof(TaskSource))]
    public class TaskProvider : MonoBehaviour
    {
        [Header("Przedmioty wymagane do wykonania polecenia")]
        [SerializeField]
        private ItemModel[] _itemModels;

        [Header("Ilości tych przedmiotów")]
        [SerializeField]
        private float[] _itemAmounts;

        [SerializeField]
        private ToolType _toolType;

        private TaskSource _taskSource;

        private TaskSource TaskSource =>
            _taskSource ??= GetComponent<TaskSource>();

        #region Unity

        private void OnValidate()
        {
            if (_itemModels.Length != _itemAmounts.Length)
                Debug.LogError("ItemModels and ItemAmounts muszą posiadać tyle" +
                    "samo elementów");
        }

        #endregion Unity

        #region Private

        private Task GetTask()
        {
            return new(
                _itemModels.Select(model => model.Id).ToArray(),
                _itemAmounts,
                (int)_toolType,
                0,
                0,
                0);
        }

        #endregion Private
    }
}