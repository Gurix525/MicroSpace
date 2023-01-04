using System.Linq;
using Entities;
using Miscellaneous;
using ScriptableObjects;
using UnityEngine;

namespace Tasks
{
    [RequireComponent(typeof(TaskSource))]
    public abstract class TaskProvider : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        protected ToolType _toolType;

        private TaskSource _taskSource;

        private Task _task;

        #endregion Fields

        #region Properties

        protected TaskSource TaskSource =>
            _taskSource ??= GetComponent<TaskSource>();

        protected Task Task =>
            _task ??= GetTask();

        #endregion Properties

        #region Private

        protected abstract Task GetTask();

        protected abstract void Execute();

        #endregion Private
    }
}